# trident
Utility to upload local and network folders data to aws s3 bucket.
Supported on Windows environment.

### Pre-requisite
1. Windows 7 or older 
2. Visual Studio 2017 Community [download link](https://visualstudio.microsoft.com/vs/community/) (free!)
3. .NET Framework 4.6 or older
4. An AWS account [(link)](https://aws.amazon.com/)

> **Note**: You are responsilble for any cost associated with using AWS services. 

> **Note**: You are also responsible to configure security on your AWS account to protect your data on AWS.
### Compile
Install Visual Studio 2017 (VS) on Win 7 or later.
Make sure .NET Framework 4.6 or later is installed.
Clone or download the souce code zip from this github project [(download link)](https://github.com/jabbar-gabbar/trident/archive/master.zip) and compile it using VS. The complier will generate app binaries.

### AWS setup
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
