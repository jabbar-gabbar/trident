# trident
A tool to automatically upload/backup/sync local or network folder data to Amazon AWS S3 (Simple Storage Service) bucket(s). The trident is supported only on Windows OS.

Trident can be used to backup personal media files and folders so that it can be useful during failures of main external hard drives or network attach storage or computer disks. Important to note that data uploaded by trident are one-way sync, meaning it remains in the S3 bucket until the user deletes the data manually in S3, that way if a file is deleted from a computer or external hard drives, it can still be recovered manually from the S3.

Important to note that a file is only uploaded one time to S3 when it is first discovered by the trident. The same file will not be uploaded again in a next run because the local inventory file keeps track of uploaded files. This can make a good use case for uploading pictures and videos which often do not change after its creation. It may not be suitable for files such as text file or PDF which can be changed after its creation.

### Use Cases
Following requirements make good use cases:
1. Off-site long term data backup
2. Automate data backup via task scheduler
3. Reduce cost and maintenance of local or network storage by deleting uploaded data
4. Protection against hardware failure or ransomware 

Data is stored in S3 bucket of your AWS account.  You will pay only for what you use.  S3 pricing is explained in detail [link](https://aws.amazon.com/s3/pricing). You can further reduce the cost of S3 by moving objects to AWS Glacier by using lifecycle policies on a S3 bucket.

### Prerequisite
1. Windows 7 or older 
2. Visual Studio 2017 Community edition (free!)
3. .NET Framework 4.6 or older.  .NET Core is not supported.
4. AWS account [(link)](https://aws.amazon.com/)

> **Important**: You are responsible for any cost associated with using AWS services. 

> **Note**: You are responsible to configure security as per the best practices on your AWS account to protect your data in AWS.
### Software and Tools Setup
- Install Visual Studio 2017 (VS) community edition [(link)](https://visualstudio.microsoft.com/vs/community/).
- Install .NET Framework 4.6 or latest [(link)](https://dotnet.microsoft.com/download/dotnet-framework). 
- Clone or download the source code zip from this github project [(link)](https://github.com/jabbar-gabbar/trident/archive/master.zip).

### AWS setup
Trident uses AWS S3 APIs to upload files to AWS. 
[AWS setup is docomented here in detail.](docs/AWS-Setup.md)

### Compile and Build 
### Deploy and Configure
### Troubleshooting and Maintanance
### Schedule the auto sync
Useful command:

dir \\myserver\iphone /o:-s/b/s