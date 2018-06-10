Write-Host "No spaces in your paths, or the generation will fail"
$googleapisdir = Read-Host 'Path to the https://github.com/googleapis/googleapis repository. Example: c:\googleapis\'
$tooldir = Read-Host 'Path to the grpc tools folder. Example: c:\yourProject\packages\Grpc.Tools.1.10.0\tools\windows_x86\'
$outdir = Read-Host 'Output dir'

$dirchar = [System.IO.Path]::DirectorySeparatorChar
$googledir = [System.IO.Path]::Combine($googleapisdir, "google")
$assistantdir = [System.IO.Path]::Combine($googledir, "assistant", "embedded", "v1alpha2")
$srcdir = [System.IO.Path]::Combine($googleapisdir, "third_party", "protobuf", "src")

$protoc = [System.IO.Path]::Combine($tooldir,"protoc.exe")
$grpc_csharp_plugin = [System.IO.Path]::Combine($tooldir, "grpc_csharp_plugin.exe")

# parameter for code generation
$params = "-I$srcdir", "-I.", "-I$googleapisdir", "--csharp_out=$outdir$dirchar", "--grpc_out=$outdir$dirchar", "--plugin=protoc-gen-grpc=$grpc_csharp_plugin"

# needed files for the googleassistant are from https://github.com/vanshg/MacAssistant/blob/master/gen_swift_proto.sh
$params_annotations = $params + [System.IO.Path]::Combine($googledir, "api", "annotations.proto")
$params_http = $params + [System.IO.Path]::Combine($googledir, "api", "http.proto")
$params_latlng = $params + [System.IO.Path]::Combine($googledir, "type", "latlng.proto")
$params_status = $params + [System.IO.Path]::Combine($googledir, "rpc", "status.proto")
$params_embedded_assistant = $params + [System.IO.Path]::Combine($assistantdir, "embedded_assistant.proto")

Write-Host "generating Annotations.cs"
& $protoc $params_annotations

Write-Host "generating Http.cs"
& $protoc $params_http

Write-Host "generating Latlng.cs"
& $protoc $params_latlng

Write-Host "generating Status.cs"
& $protoc $params_status

Write-Host "generating EmbeddedAssistant.cs"
& $protoc $params_embedded_assistant

Write-Host "Done"
pause