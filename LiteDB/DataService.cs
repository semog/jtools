﻿using System;
using System.IO;

namespace LiteDB
{
    internal class DataService
    {
        private PageService _pager;

        public DataService(PageService pager)
        {
            _pager = pager;
        }

        /// <summary>
        /// Insert data inside a datapage. Returns dataPageID that idicates the first page
        /// </summary>
        public DataBlock Insert(CollectionPage col, byte[] data)
        {
            // set collection page as dirty before changes
            _pager.SetDirty(col);

            // need to extend (data is bigger than 1 page)
            var extend = (data.Length + DataBlock.DATA_BLOCK_FIXED_SIZE) > BasePage.PAGE_AVAILABLE_BYTES;

            // if extend, just search for a page with BLOCK_SIZE avaiable
            var dataPage = _pager.GetFreePage<DataPage>(col.FreeDataPageID, extend ? DataBlock.DATA_BLOCK_FIXED_SIZE : data.Length + DataBlock.DATA_BLOCK_FIXED_SIZE);

            // create a new block with first empty index on DataPage
            var block = new DataBlock { Position = new PageAddress(dataPage.PageID, dataPage.DataBlocks.NextIndex()), Page = dataPage };

            // if extend, store all bytes on extended page.
            if (extend)
            {
                var extendPage = _pager.NewPage<ExtendPage>();
                block.ExtendData = data;
                block.ExtendPageID = extendPage.PageID;
                this.StoreExtendData(extendPage, data);
            }
            else
            {
                block.Data = data;
            }

            // add dataBlock to this page
            dataPage.DataBlocks.Add(block.Position.Index, block);

            // update freebytes + items count
            dataPage.UpdateItemCount();

            // add/remove dataPage on freelist if has space
            _pager.AddOrRemoveToFreeList(dataPage.FreeBytes > DataPage.DATA_RESERVED_BYTES, dataPage, col, ref col.FreeDataPageID);

            col.DocumentCount++;

            return block;
        }

        /// <summary>
        /// Update data inside a datapage. If new data can be used in same datapage, just update. Otherside, copy content to a new ExtendedPage
        /// </summary>
        public DataBlock Update(CollectionPage col, PageAddress blockAddress, byte[] data)
        {
            // get datapage and mark as dirty
            var dataPage = _pager.GetPage<DataPage>(blockAddress.PageID, true);
            var block = dataPage.DataBlocks[blockAddress.Index];
            var extend = dataPage.FreeBytes + block.Data.Length - data.Length <= 0;

            // check if need to extend
            if (extend)
            {
                // clear my block data
                block.Data = new byte[0];
                block.ExtendData = data;

                // create (or get a existed) extendpage and store data there
                ExtendPage extendPage;

                if (block.ExtendPageID == uint.MaxValue)
                {
                    extendPage = _pager.NewPage<ExtendPage>();
                    block.ExtendPageID = extendPage.PageID;
                }
                else
                {
                    extendPage = _pager.GetPage<ExtendPage>(block.ExtendPageID, true);
                }

                this.StoreExtendData(extendPage, data);
            }
            else
            {
                // if no extends, just update data block
                block.Data = data;

                // if there was a extended bytes, delete
                if (block.ExtendPageID != uint.MaxValue)
                {
                    _pager.DeletePage(block.ExtendPageID, true);
                    block.ExtendPageID = uint.MaxValue;
                }
            }

            // updates freebytes + items count
            dataPage.UpdateItemCount();

            // add/remove dataPage on freelist if has space AND its on/off free list
            _pager.AddOrRemoveToFreeList(dataPage.FreeBytes > DataPage.DATA_RESERVED_BYTES, dataPage, col, ref col.FreeDataPageID);

            return block;
        }

        /// <summary>
        /// Read all data from datafile using a pageID as reference. If data is not in DataPage, read from ExtendPage. If readExtendData = false, do not read extended data
        /// </summary>
        public DataBlock Read(PageAddress blockAddress, bool readExtendData)
        {
            var page = _pager.GetPage<DataPage>(blockAddress.PageID);
            var block = page.DataBlocks[blockAddress.Index];

            // if there is a extend page, read bytes to block.Data
            if (readExtendData && block.ExtendPageID != uint.MaxValue)
            {
                block.ExtendData = this.ReadExtendData(block.ExtendPageID);
            }

            return block;
        }

        /// <summary>
        /// Read all data from a extended page with all subsequences pages if exits
        /// </summary>
        public byte[] ReadExtendData(uint extendPageID)
        {
            // read all extended pages and build byte array
            using (var buffer = new MemoryStream())
            {
                foreach (var extendPage in _pager.GetSeqPages<ExtendPage>(extendPageID))
                {
                    buffer.Write(extendPage.Data, 0, extendPage.Data.Length);
                }

                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Delete one dataBlock
        /// </summary>
        public DataBlock Delete(CollectionPage col, PageAddress blockAddress)
        {
            // get page and mark as dirty
            var page = _pager.GetPage<DataPage>(blockAddress.PageID, true);
            var block = page.DataBlocks[blockAddress.Index];

            // mark collection page as dirty
            _pager.SetDirty(col);

            // if there a extended page, delete all
            if (block.ExtendPageID != uint.MaxValue)
            {
                _pager.DeletePage(block.ExtendPageID, true);
            }

            // delete block inside page
            page.DataBlocks.Remove(block.Position.Index);

            // update freebytes + itemcount
            page.UpdateItemCount();

            // if there is no more datablocks, lets delete the page
            if (page.DataBlocks.Count == 0)
            {
                // first, remove from free list
                _pager.AddOrRemoveToFreeList(false, page, col, ref col.FreeDataPageID);

                _pager.DeletePage(page.PageID);
            }
            else
            {
                // add or remove to free list
                _pager.AddOrRemoveToFreeList(page.FreeBytes > DataPage.DATA_RESERVED_BYTES, page, col, ref col.FreeDataPageID);
            }

            col.DocumentCount--;

            return block;
        }

        /// <summary>
        /// Store all bytes in one extended page. If data ir bigger than a page, store in more pages and make all in sequence
        /// </summary>
        public void StoreExtendData(ExtendPage page, byte[] data)
        {
            var offset = 0;
            var bytesLeft = data.Length;

            while (bytesLeft > 0)
            {
                var bytesToCopy = Math.Min(bytesLeft, BasePage.PAGE_AVAILABLE_BYTES);

                page.Data = new byte[bytesToCopy];

                Buffer.BlockCopy(data, offset, page.Data, 0, bytesToCopy);

                // updates free bytes + items count
                page.UpdateItemCount();

                bytesLeft -= bytesToCopy;
                offset += bytesToCopy;

                // if has bytes left, let's get a new page
                if (bytesLeft > 0)
                {
                    // if i have a continuous page, get it... or create a new one (set as dirty)
                    page = page.NextPageID != uint.MaxValue ?
                        _pager.GetPage<ExtendPage>(page.NextPageID, true) :
                        _pager.NewPage<ExtendPage>(page);
                }
            }

            // when finish, check if last page has a nextPageId - if have, delete them
            if (page.NextPageID != uint.MaxValue)
            {
                // Delete nextpage and all nexts
                _pager.DeletePage(page.NextPageID, true);

                // set my page with no NextPageID
                page.NextPageID = uint.MaxValue;
            }
        }
    }
}