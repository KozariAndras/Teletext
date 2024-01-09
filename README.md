# Teletext Application

## Description
The Teletext Application is a program designed to manage television program schedules. It allows users to add, organize, and view TV program details efficiently. The application caters to both administrative and user functionalities, facilitating program addition, statistics generation, and export features.

## Admin Interface:
- Add a new program (name, time, duration, age restriction, channel, genre)
- Create statistics based on program categories for specific days (e.g., weekdays or weekends). The statistics can be based on time proportions or the number of programs. Display the results in a pie chart.

## User Interface:
- Mark programs that users want to watch. View the marked programs separately on another page.
- Retrieve all programs sorted by date, channel, and genre.
- Export programs in XML format on a weekly basis for easy reference.

## Installation
1. Clone the repository: `git clone https://github.com/KozariAndras/Teletext.git`
2. Navigate to the project directory: `cd Teletext`
3. Ensure you have ASP .NET and .NET 7 installed on your system.
4. Open the project in your preferred IDE or editor compatible with ASP .NET and .NET 7.
5. Set up the database and migrations using Microsoft Entity Framework.
6. Build and run the application using the respective commands or IDE tools.

## Usage
- Access the application via the provided URL or after starting the local server.
- Use the Admin interface to add programs and generate statistics.
- Use the User interface to mark programs, view program lists, and export in XML format.

## Technologies Used
- .NET 7
- ASP .NET
- Microsoft Entity Framework

## Note
- This project is currently under active development. Some features might be incomplete or subject to change.