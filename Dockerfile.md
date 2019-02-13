# Using Docker instances

This is showing the SDK, you'd want to use the runtime for most things.

```
FROM microsoft/dotnet:2.2-sdk-alpine3.8 AS alpine

# This is required for libld to load the package
# The copying moves the libzstd to the standard library location
RUN apk add --no-cache libc6-compat && \
    cp linuxalpine/libzstd.so /usr/lib/

WORKDIR /src
COPY ./ ./

# Entry point etc 
```

```
### Note: MSFT default image "sdk" is stretch https://github.com/dotnet/dotnet-docker
FROM microsoft/dotnet:2.2-sdk-stretch AS stretch

# Installing the lib is pointless as its 1.2 and fails the dictionary tests
RUN apt-get update \
    && apt-get install -y --no-install-recommends libc6-dev \
    && cp linuxdebian/libzstd.so /usr/lib/
    
WORKDIR /src
COPY ./ ./

# Entry point etc 
```

```
### bionic
FROM microsoft/dotnet:2.2-sdk-bionic AS bionic

# libzstd1 is 1.3.3 which passes the tests
RUN apt-get update \
    && apt-get install -y --no-install-recommends libc6-dev \
    && cp linuxdebian/libzstd.so /usr/lib/
        
WORKDIR /src
COPY ./ ./

# Entry point etc 
```
