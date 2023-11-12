docker-compose up -d

aws --endpoint-url=http://localhost:4566 --profile LocalStack s3api create-bucket --bucket ods-table-data

aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/RWSSettings/RWSServer" --type String --value "https://theradex.mdsol.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/RWSSettings/RWSUserName" --type String --value "SPEC_TRACK_RWS" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/RWSSettings/RWSPassword" --type String --value "Password@01" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/RWSSettings/TimeoutInSecs" --type String --value "3600" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/EmailSettings/FromAddress" --type String --value "dev-noreply@theradex.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/EmailSettings/ToAddress" --type String --value "uvarada@theradex.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/AppSettings/ArchiveBucket" --type String --value "ods-table-data" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/AppSettings/LocalArchivePath" --type String --value "C:\\ODS\\Manager\\Data" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/ODSSettings/Host" --type String --value "database-integrations-ods01-instance-1.csin3dm2hdsj.us-east-1.rds.amazonaws.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/ODSSettings/Port" --type String --value "5432" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/ODSSettings/Username" --type String --value "postgres" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/ODSSettings/Password" --type String --value "Password001" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/ODSSettings/TimeoutInSecs" --type String --value "60" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsManager/ODSSettings/Database" --type String --value "ods" --overwrite
pause