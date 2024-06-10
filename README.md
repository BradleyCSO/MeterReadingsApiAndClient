# Introduction
This project contains a minimal API, a client (React with Vite) that consumes it, as well as a test project for the minimal API containing various test cases asserting its expected functionality, using Moq and xUnit.

The API itself allows for the insertion of meter reading data, which is then persisted to a PostgreSQL database. The client consumes this endpoint, providing a form component which allows for the upload of a .csv file, which the API then handles.

# Setup (With Docker)
This project has a <code>docker-compose</code> file which containerises a PostgreSQL database instance and pgAdmin alongside it, which both can be run with the following commands:  

<code>docker-compose build</code>

<code>docker-compose up</code>

From there, run the projects can be run locally, pointing to the PostgreSQL database instance running in the Docker container.

## pgAdmin: Logging In
Now that the Docker container is running, we  should have a pgAdmin instance. The login credentials for this are defined in the <code>docker-compose.yml</code> file.

# Setup (Without Docker)
To setup this project without Docker, the only dependency for this project is PostgreSQL which can be downloaded from: https://www.postgresql.org/download/, from there a localhost server can be setup, where the <code>appsettings.json</code> Postgres connection string can be updated to point to this.