Okay look this project is related to content moderation app. It uses the Azure AI Content Safety feature to determine whethr some text and image contains some unwanted content or not by taking into consideration four parameters (Violence,Sexual,Self Harm and Hate).Users can login and Submit their text and image for verfication and it will judge the severity off the submission.Apart from this its other feautures are that a Admin user can View all submissions and can manually unflag them.
How this app will be run => This app uses local db. You have to sign it to azure portal => Create an Content Safety resource => Copy and get its Endpoint and key => Paste them in the appsettings.json.
For reviewing app as an admin role user an Admin role is seeded into the application Email = "admin@yourapp.com", Password = "Admin@123".
Please install the packages required for it before running it all mentioned in the project file.
And sorry but I prefferred to write this readme file by myself thats why you have to read my poor english.
In a nutsell 
Backend = ASP.NET MVC, SQL server Database
Frontend = HTML, CSS, Javascript
Features => Role based authentication and authorization, and whatever i have written up.
Thanks, BY THE WAY!!
