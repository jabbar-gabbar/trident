# AWS setup
Trident uses AWS S3 APIs to upload files to AWS.  In order for Trident to be able to upload date to a S3 bucket in your AWS account,  you will need to provide a credential file.  

#### Create AWS account
Creating AWS account is described in detail in this [link](https://aws.amazon.com/premiumsupport/knowledge-center/create-and-activate-aws-account/).

When you first create an account you begin with a single sigh-in identity that has complete access to all AWS services,  the identity is called *root user*.  It is usually your email address by which you first created the AWS account.  The details of root user is given here in [detail](https://docs.aws.amazon.com/IAM/latest/UserGuide/id_root-user.html).

The root user has highest priviledges and hence it is your duty to protect the root user.  You should not use root user for everyday tasks, instead you should create an IAM user.  Follow the best practices as described in details [here](https://docs.aws.amazon.com/IAM/latest/UserGuide/best-practices.html).

It is good practice to enable multi-factor authentication for your root user or IAM user as described [here](https://docs.aws.amazon.com/IAM/latest/UserGuide/id_credentials_mfa.html). 

#### Create AWS IAM User Account
IAM user is another user that is separate from root user and associated to your AWS account.  You can create one or more IAM users for differnt tasks.  You can give certain or limited permission to the IAM user, that way you can protect your AWS account by giving least priviledges that is needed to perform only certail actions. 
Trident will need IAM user's Access Key to access your S3 bucket(with read/list and write actions) programatically via S3 APIs.  
Create a new IAM user by following steps in this [link](https://docs.aws.amazon.com/IAM/latest/UserGuide/id_users_create.html).
You will need to select **Programmatic access** as type of access while creating user.  This will create Access Key which you can download or view on the **Final** page. 

#### Create AWS S3 buckets
You will need to create an S3 bucket to store files you want to backup.  You can create more than one S3 buckets if you want to store data in separate buckets.  Trident supports that scenario. 
Quick steps are given [here](https://docs.aws.amazon.com/quickstarts/latest/s3backup/step-1-create-bucket.html).
#### Grant IAM User S3 permission through Policy
Once you have an IAM user with Access Key and a S3 bucket,  you need to grant IAM user rights to upload and list data in the S3 bucket so that when Trident runs using the Access Key it has sufficient permission to list the files in the bucket and put the files in the bucket. 
You can do that by creating a policy that describes the permission to specific S3 bucket.  You can then attach the policy to IAM user.  
You can perform this in IAM in AWS [console(us-east-1)](https://console.aws.amazon.com/iam/home?region=us-east-1#/policies) and click Create Policy.
Example of the Policy in JSON:
```JSON
	{
		"Version": "2012-10-17",  
		"Statement": [
		{
		  "Sid": "AllowListBucketIfSpecificPrefixIsIncludedInRequest",
		  "Action": [
				"s3:GetObjectVersionTagging",
                "s3:ReplicateObject",
                "s3:PutObjectTagging",
                "s3:ListMultipartUploadParts",
                "s3:PutObject",
                "s3:GetObject",
                "s3:ListBucketByTags",
                "s3:GetBucketTagging",
                "s3:ListBucketVersions",
                "s3:ListBucket",
                "s3:AbortMultipartUpload",
                "s3:PutBucketTagging",
                "s3:GetObjectTagging",
                "s3:PutBucketVersioning",
                "s3:ListBucketMultipartUploads",
                "s3:PutObjectVersionTagging",
                "s3:GetBucketVersioning",
                "s3:GetObjectVersion"
		  ],
		  "Effect": "Allow",
		  "Resource": [
				"arn:aws:s3:::mys3bucketname",
				"arn:aws:s3:::mys3bucketname/*",
				"arn:aws:s3:::mys3bucketname2",
				"arn:aws:s3:::mys3bucketname2/*"  
		  ]
		   }
		}
	  ]
	}
```

#### Generate Access Keys and credential file
#### Store Credential file
Run the following commands to create aws credential file in 'C:\users\{myuser}\.aws' folder.
Open cmd window

    cd %HOMEPATH%

    mkdir .aws

    cd .aws

    copy NUL credentials

enter credentials in the file in following format. 


                