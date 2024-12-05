FROM alpine:latest
WORKDIR /app

RUN apk add --no-cache \
    unzip \
    ca-certificates \
    build-base

# download and unzip PocketBase
ADD https://go.dev/dl/go1.22.3.linux-amd64.tar.gz /tmp/go.tar.gz 
RUN tar -C /usr/local -xzf /tmp/go.tar.gz
RUN export PATH=$PATH:/usr/local/go/bin

# uncomment to copy the local pb_migrations dir into the image
COPY . . 
RUN /usr/local/go/bin/go build -tags "sqlite_fts5 sqlite_json sqlite_foreign_keys sqlite_vtable sqlite_math_functions"
RUN ./myapp migrate --dev
RUN ./myapp migrateimages --dev
RUN ./myapp migratestatics --dev

EXPOSE 8080

# start PocketBase
CMD ["./myapp", "serve", "--http=0.0.0.0:8080"]
