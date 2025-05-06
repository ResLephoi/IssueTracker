# Issue Tracker

This project is an Issue Tracker application that consists of a client-side Angular application and a server-side Node.js application. The application allows users to manage tasks, including creating, editing, and deleting cards on a board.

## Project Structure

```
IssueTracker
├── client                # Client-side application
│   ├── src               # Source files for the Angular app
│   ├── angular.json      # Angular CLI configuration
│   ├── package.json      # NPM dependencies for the client
│   ├── tsconfig.json     # TypeScript configuration for the client
│   └── README.md         # Documentation for the client application
└── server                # Server-side application
    ├── src               # Source files for the Node.js app
    ├── package.json      # NPM dependencies for the server
    ├── tsconfig.json     # TypeScript configuration for the server
    └── README.md         # Documentation for the server application
```

## Client Application

The client application is built using Angular and provides a user interface for managing tasks. It includes features such as:

- Viewing tasks on a board
- Adding new tasks
- Editing existing tasks
- Deleting tasks

### Setup Instructions

1. Navigate to the `client` directory.
2. Install dependencies:
   ```
   npm install
   ```
3. Run the application:
   ```
   ng serve
   ```

## Server Application

The server application is built using Node.js and Express. It provides an API for managing tasks and interacts with a database to store task data.

### Setup Instructions

1. Navigate to the `server` directory.
2. Install dependencies:
   ```
   npm install
   ```
3. Run the application:
   ```
   npm start
   ```

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any suggestions or improvements.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.