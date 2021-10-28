# AlterEgo-core

This is an queueing system for [AlterEgo-core](https://github.com/KMielnik/AlterEgo-core), which allows you to create deep fakes easily.
AlterEgo-server allows you to register your account, use it to upload your files, mange them, and create deepfakes with REST API, which can be consumed by build in Swagger or by mobile application: [AlterEgo](https://github.com/KMielnik/Alterego).



## How to use

### Building with Docker Compose (Linux/WSL heavilly recommended)

1. Clone the repository
1. Configure ```docker-compose.yml```
   1. Set ```ALTEREGO_*_FOLDER``` locations and their volumins to point at real folders in your filesystem
   1. Set ```ASPNETCORE_Kestrel__Certificates__Default__*``` to point at your https certificate and enter its password
   1. Set Docker socket volumin (```/var/run/docker.sock:/var/run/docker.sock``` by default)
   1. [Optional - needed for working Firebase Messaging] Set config file with ```GOOGLE_APPLICATION_CREDENTIALS``` and put those files in https volumin
   1. [Otpional - not recommended] If CUDA GPU is not present, disable it by setting ```ALTEREGO_GPU_SUPPORT``` to ```N``` and set the ```UsingGPU``` parameter in ```appsettings.json``` to ```false```
1. Run ```docker-compose up```
1. Open API to use it via Swagger or connect AlterEgo app
