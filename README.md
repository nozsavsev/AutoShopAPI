When running frontend, please supply 
`API_URL=` in .env or environment.

Remarks for frontend:
I do understand that stuffing react-router into nextjs is a textbook-bad idea, however, I've run into some issues creating and deploying a standalone project in the given time, so I decided to go with this, as project setup is important, but by far not the only thing here.

Remarks for backend: 
There are some CORS issues when deployed in Docker, so to host it, I had to hardcode some values. Again, I know it's bad don't have time to fix it.

Usually, I would exclude appsettings.json from the project or simply not store connection strings in there; however, for the sake of making life easier, since there is no actual sensitive information, I left it here

When running, please make sure the database is up and running before api starts. Otherwise, it will fail to apply migrations and die
