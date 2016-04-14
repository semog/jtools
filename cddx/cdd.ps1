cddx $args
if($lastexitcode -eq 1)
{
	$private:changedirscript = "$env:localappdata\powertools\changedir.ps1"
	if(test-path $private:changedirscript)
	{
		. $private:changedirscript
		remove-item $private:changedirscript | out-null
	}
}
