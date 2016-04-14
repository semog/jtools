pddx $args
if($lastexitcode -eq 1)
{
	$private:popdirscript = "$env:localappdata\powertools\popdir.ps1"
	if(test-path $private:popdirscript)
	{
		. $private:popdirscript
		remove-item $private:popdirscript | out-null
	}
}
