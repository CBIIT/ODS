docker-compose up -d

aws --endpoint-url=http://localhost:4566 --profile LocalStack s3api create-bucket --bucket ods-table-data

aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/RWSSettings/RWSServer" --type String --value "https://theradex.mdsol.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/RWSSettings/RWSUserName" --type String --value "SPEC_TRACK_RWS" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/RWSSettings/RWSPassword" --type String --value "Password@01" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/RWSSettings/TimeoutInSecs" --type String --value "3600" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/EmailSettings/FromAddress" --type String --value "dev-noreply@theradex.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/EmailSettings/ToAddress" --type String --value "uvarada@theradex.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/AppSettings/ArchiveBucket" --type String --value "ods-table-data" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/AppSettings/LocalArchivePath" --type String --value "C:\\ODS\\Extractor\\Data" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/ODSSettings/Host" --type String --value "ods-instance-1.cgntten8b01g.us-west-1.rds.amazonaws.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/ODSSettings/Port" --type String --value "5432" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/ODSSettings/Username" --type String --value "postgres" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/ODSSettings/Password" --type String --value "Password0001" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/ODSSettings/TimeoutInSecs" --type String --value "60" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/ODSSettings/Database" --type String --value "ods" --overwrite



aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/RWSSettings/RWSServer" --type String --value "https://theradex.mdsol.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/RWSSettings/RWSUserName" --type String --value "SPEC_TRACK_RWS" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/RWSSettings/RWSPassword" --type String --value "Password@01" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/RWSSettings/TimeoutInSecs" --type String --value "3600" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/EmailSettings/FromAddress" --type String --value "dev-noreply@theradex.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/EmailSettings/ToAddress" --type String --value "uvarada@theradex.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/AppSettings/ArchiveBucket" --type String --value "ods-table-data" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/AppSettings/LocalArchivePath" --type String --value "C:\\ODS\\Manager\\Data" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/ODSSettings/Host" --type String --value "ods-instance-1.cgntten8b01g.us-west-1.rds.amazonaws.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/ODSSettings/Port" --type String --value "5432" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/ODSSettings/Username" --type String --value "postgres" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/ODSSettings/Password" --type String --value "Password0001" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/ODSSettings/TimeoutInSecs" --type String --value "60" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsmanager/ODSSettings/Database" --type String --value "ods" --overwrite



aws --endpoint-url=http://localhost:4566 --profile LocalStack dynamodb create-table --table-name odsmanager_table_metadata --attribute-definitions AttributeName=id,AttributeType=N --key-schema AttributeName=id,KeyType=HASH --provisioned-throughput ReadCapacityUnits=10,WriteCapacityUnits=10
aws --endpoint-url=http://localhost:4566 --profile LocalStack dynamodb create-table --table-name batch_run_control --attribute-definitions AttributeName=id,AttributeType=N --key-schema AttributeName=id,KeyType=HASH --provisioned-throughput ReadCapacityUnits=10,WriteCapacityUnits=10

aws  --endpoint-url=http://localhost:4566 --profile LocalStack dynamodb create-table \
                                                                            --table-name ProductReview \
                                                                            --attribute-definitions \
                                                                                AttributeName=UserId,AttributeType=N \
                                                                                AttributeName=ProductName,AttributeType=S \
                                                                            --key-schema \
                                                                                AttributeName=UserId,KeyType=HASH \
                                                                                AttributeName=ProductName,KeyType=RANGE \
                                                                            --provisioned-throughput \
                                                                                ReadCapacityUnits=5,WriteCapacityUnits=5

aws --endpoint-url=http://localhost:4566 --profile LocalStack dynamodb list-tables
aws --endpoint-url=http://localhost:4566 --profile LocalStack s3 ls s3://ods-table-data --recursive 
REM Delete all files from bucket and all folders recursive
REM aws --endpoint-url=http://localhost:4566 --profile LocalStack s3 rm --recursive s3://ods-table-data   

REM Delete the bucket
REM aws --endpoint-url=http://localhost:4566 --profile LocalStack s3 rb --force s3://your_bucket_name

REM aws s3 sync  s3://ods-table-data/dev s3://ods-table-data/uat  --profile default

pause