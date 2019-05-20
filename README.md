# trident
A tool to upload/backup/sync local or network folder data to Amazon AWS S3 (Simple Storage Service) bucket(s). Supported on Windows environment only.

Trident can be used to backup personal media files and folders so that it can be useful during failures of main external hard drives or network attach storage or computer disks. Data uploaded by trident is one-way sync meaning it remains in s3 bucket until user deletes the data manually in s3, that way if a file is deleted from computer or external hard drives, it can be recovered from the s3.

Important to note that a file is only uploaded one time to s3 when it is first discovered by trident. The same file will not be uploaded again in next run as the local inventory keeps track of uploaded files. This can make a good use case for uploading pictures and videos which often do not change. It may not be suitable for files such as text file or PDF which can be changed after its creation.

Following requirements makes good use case:
1. Offsite long term data backup
2. Automate data backup via task schedular
3. Reduce cost and maintanance of local or network storage by deleting uploaded data
4. Protection against hardware failure or ransomware 

Data is stored in your aws s3 account.  You will pay only for what you use.  S3 pricing is explained in detail [link](https://aws.amazon.com/s3/pricing)

### Prerequisite
1. Windows 7 or older 
2. Visual Studio 2017 Community edition (free!)
3. .NET Framework 4.6 or older.  .NET Core is not supported.
4. AWS account [(link)](https://aws.amazon.com/)

> **Important**: You are responsilble for any cost associated with using AWS services. 

> **Note**: You are responsible to configure security as per the best practices on your AWS account to protect your data in AWS.
### Software setup
- Install Visual Studio 2017 (VS) community edition [(link)](https://visualstudio.microsoft.com/vs/community/).
- Install .NET Framework 4.6 or latest [(link)](https://dotnet.microsoft.com/download/dotnet-framework). 
- Clone or download the souce code zip from this github project [(link)](https://github.com/jabbar-gabbar/trident/archive/master.zip) and compile it using VS. The complier will generate app binaries.

### AWS setup
Trident uses aws s3 API to upload objects(files) to aws. 
#### Create aws credential file
#### Store cred file
Run the following commands to create aws credential file in 'C:\users\{myuser}\.aws' folder.
Open cmd window

    cd %HOMEPATH%

    mkdir .aws

    cd .aws

    copy NUL credentials

enter credentials in the file in following format. 

### Build the code
### Deploy the build
### Configure settings
### Schedule the auto sync
Useful command:

dir \\myserver\iphone /o:-s/b/s