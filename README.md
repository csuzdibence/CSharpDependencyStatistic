# CSharpDependencyStatistic

This application analyses an already existing C# solution's components by abstractness and stability.

## Prerequisites

* Firstly you must **build** the solution you want to analyse
* You have to set an environment variable ``` set SOLUTION_PATH_FOLDER=folder_with_your_built_solution ```
* You can pull the OCI image from [Docker Hub](https://hub.docker.com/repository/docker/csuzdibence/csharp-dependency-statistic/general)
* After you have the image you can run with ``` docker-compose run csharp-dependency-statistic "YourSolutionFileName.sln" ```
  - You must also give a parameter to the container with the name of your solution file

## After you start the application

There will be some time to analyse all the projects in the solution after that a png will be created with a plot and also a json file will be created with the analysed projects' data
