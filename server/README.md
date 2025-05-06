# Server Application README

# Issue Tracker Server

This is the server-side application for the Issue Tracker project. It is built using Node.js and Express, providing a RESTful API for managing boards, lists, and cards.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Contributing](#contributing)
- [License](#license)

## Installation

1. Clone the repository:
   ```
   git clone https://github.com/ResLephoi/IssueTracker.git
   ```

2. Navigate to the server directory:
   ```
   cd IssueTracker/server
   ```

3. Install the dependencies:
   ```
   npm install
   ```

## Usage

To start the server, run the following command:
```
npm start
```

The server will run on `http://localhost:3000` by default. You can change the port in the `app.ts` file.

## API Endpoints

- `GET /api/boards` - Retrieve all boards
- `POST /api/boards` - Create a new board
- `GET /api/boards/:id` - Retrieve a specific board by ID
- `PUT /api/boards/:id` - Update a specific board by ID
- `DELETE /api/boards/:id` - Delete a specific board by ID

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any improvements or bug fixes.

## License

This project is licensed under the MIT License. See the LICENSE file for details.