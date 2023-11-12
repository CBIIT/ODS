aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 993530973844.dkr.ecr.us-east-1.amazonaws.com

ods\src > docker build -f "ods.manager/Dockerfile" -t dev-theradexodsmanager . 

docker tag dev-theradexodsManager:latest 993530973844.dkr.ecr.us-east-1.amazonaws.com/dev-theradexodsmanager:dev-latest

docker push 993530973844.dkr.ecr.us-east-1.amazonaws.com/dev-theradexodsmanager:dev-latest