Remove-Item -Recurse -Force tools\mounts\azuracast
Remove-Item -Recurse -Force tools\mounts\icecast2
Copy-Item -Recurse -Force tools\mounts\init\* tools\mounts
