FROM mcr.microsoft.com/mssql/server:2022-latest

USER root
RUN apt-get update && \
    ACCEPT_EULA=Y apt-get install -y msodbcsql18 mssql-tools && \
    echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> /etc/profile
    
USER mssql
COPY ./init-db /var/opt/mssql/backup
EXPOSE 1433

CMD /bin/bash -c "\
/opt/mssql/bin/sqlservr & \
sleep 15 && \
if [ -f /var/opt/mssql/backup/BookstoreDb.bak ]; then \
  /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P \"$SA_PASSWORD\" -Q \"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'BookstoreDb') \
    BEGIN RESTORE DATABASE [BookstoreDb] FROM DISK = N'/var/opt/mssql/backup/BookstoreDb.bak' \
    WITH MOVE 'BookstoreDb' TO '/var/opt/mssql/data/BookstoreDb.mdf', \
         MOVE 'BookstoreDb_log' TO '/var/opt/mssql/data/BookstoreDb_log.ldf', REPLACE; END\"; \
fi && wait"
