#!/bin/sh
set -eu

dotnet restore SampleMonoRepo.DotNetApi/SampleMonoRepo.DotNetApi.csproj
exec dotnet run \
  --project SampleMonoRepo.DotNetApi/SampleMonoRepo.DotNetApi.csproj \
  --no-launch-profile \
  --urls http://0.0.0.0:21000
