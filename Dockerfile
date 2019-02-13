
## THE EXTRA PACKAGES ADDED DURING BUILD ARE NECESSARY FOR THE NATIVE LIB LIBRARY TO SUCCEED
## THIS IS THE FIX TO ALLOW THIS LIB TO WORK 
## WHILST https://github.com/mellinoe/nativelibraryloader/issues/2 is open
## AND WHILE https://github.com/dotnet/corefx/issues/32015 is open
#ENV LD_DEBUG=all can be used to see the full error (if the package management isnt used)


### BUILD IN ALPINE - TESTED PASSED
FROM microsoft/dotnet:2.2-sdk-alpine3.8 AS alpine
#zstd-libs in apk is 1.3.4
RUN apk add --no-cache libc6-compat
ENV ALPINE=TRUE
WORKDIR /src
COPY ./ ./
RUN ls /src/Zstandard.Net.Tests/bin/

RUN dotnet test


### BUILD IN STRETCH - TESTED PASSED
### Note: MSFT default image "sdk" is stretch https://github.com/dotnet/dotnet-docker
FROM microsoft/dotnet:2.2-sdk-stretch AS stretch

# Installing the lib is pointless as its 1.2 and fails the dictionary tests
RUN apt-get update \
    && apt-get install -y --no-install-recommends libc6-dev
    
WORKDIR /src
COPY ./ ./

RUN dotnet test


### BUILD IN bionic - TESTED PASSED
FROM microsoft/dotnet:2.2-sdk-bionic AS bionic

# libzstd1 is 1.3.3 which passes the tests
RUN apt-get update \
    && apt-get install -y --no-install-recommends libc6-dev
        
WORKDIR /src
COPY ./ ./

RUN dotnet test

