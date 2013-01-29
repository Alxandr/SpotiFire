param($installPath, $toolsPath, $package, $project)

$item = $project.ProjectItems | Where-Object { $_.Name -eq "libspotify.dll" }
$item.Properties.Item("CopyToOutputDirectory").Value = 1 # CopyToOutputDirectory = Copy always