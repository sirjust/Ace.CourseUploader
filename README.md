# Anytime CE Course Uploader

This program provides an automated way to upload courses, lessons, quizzes, and questions into the Anytime CE Learning Management System.

It is heavily dependent on the state of the system, ie the user interface. Due to this, even small changes to the UI can and will result in failures. A poor connection can also cause issues. Furthermore, when the browser it uses (Chrome, in this case) updates, it can also cause a failure. Thus, it will require a measure of support to function for any length of time.

## Usage

When the program starts up, it will open a Chrome window which may obscure the console. Click into the console to input your username and password. Credentials will not be remembered or embedded in the code.

The program requires an excel spreadsheet, entitled `Course Uploads.xslx` in the same directory. The first step it takes is to read this spreadsheet. The format of this spreadsheet is very specific. If it exhibits any errors while reading, consult with the programmer or project manager.

The uploader has the capability to upload only questions. The rationale behind this is that the questions are the most complex of the upload materials. The use case would be that all courses, lessons, and quizzes have completed, but one of the questions ran into an error and the program stopped.

To run only questions, type the following in the command line

```console
Ace.CourseUploader.exe -q Y
```

If using Powershell, type

```powershell
.\Ace.CourseUploader.exe -q Y
```

## License

[MIT](https://choosealicense.com/licenses/mit/)