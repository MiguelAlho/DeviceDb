# Device DB

A Device management REST API.


## How to Build

To build and work on it, clone the repository:

```
git clone https://github.com/MiguelAlho/DeviceDb
```

and from the command line, in the repository folder, run: 

```
dotnet build
```

or , to build and run tests

```
dotnet test
```

The project requires and SDK version 6.0.100-rc.1.21463.6 or above.

## How To Run

Open the `DeviceDb.sln` solution file in Visual Studio and click f5. The application 
should launch in a new console window and a browser window should open. You can navigate 
to the [https://localhost:7075/swagger](https://localhost:7075/swagger) endpoint. 

Alternatively, in a console, run:

```
> dotnet run --project .\src\DeviceDb.Api\DeviceDb.Api.csproj
```



