# Minimal client image: sqlcmd against the mssql service (reliable on any mssql server image)
FROM ubuntu:22.04

ARG DEBIAN_FRONTEND=noninteractive
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates curl apt-transport-https gnupg2 \
    && curl -sSL -O https://packages.microsoft.com/keys/microsoft.asc \
    && gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg microsoft.asc \
    && rm -f microsoft.asc \
    && echo "deb [arch=amd64,arm64,armhf signed-by=/usr/share/keyrings/microsoft-prod.gpg] https://packages.microsoft.com/ubuntu/22.04/prod jammy main" > /etc/apt/sources.list.d/microsoft-prod.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y mssql-tools18 \
    && rm -rf /var/lib/apt/lists/*

ENV PATH="/opt/mssql-tools18/bin:${PATH}"

COPY run-init.sh /usr/local/bin/run-init.sh
RUN chmod +x /usr/local/bin/run-init.sh

ENTRYPOINT ["/usr/local/bin/run-init.sh"]
