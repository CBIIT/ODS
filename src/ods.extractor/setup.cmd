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
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/ODSSettings/Host" --type String --value "database-integrations-ods01-instance-1.csin3dm2hdsj.us-east-1.rds.amazonaws.com" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/ODSSettings/Port" --type String --value "5432" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/ODSSettings/Username" --type String --value "postgres" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/ODSSettings/Password" --type String --value "Password0001" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/ODSSettings/TimeoutInSecs" --type String --value "60" --overwrite
aws --endpoint-url=http://localhost:4566 --profile LocalStack ssm put-parameter --name "/local-stack/app/odsextractor/ODSSettings/Database" --type String --value "postgres" --overwrite
pause