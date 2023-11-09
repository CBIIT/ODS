@ECHO Build for TAG %1%
@ECHO if you did not specify valid TAG then Ctrl+C and try again
pause

aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 993530973844.dkr.ecr.us-east-1.amazonaws.com
@ECHO Any errors Ctrl+C and do something to fix
pause 

docker  build --build-arg Version=%1% -f "ods.extractor/Dockerfile" -t odsextractor:%1% .
@ECHO Any errors Ctrl+C and do something to fix
pause

#docker tag odsextractor:latest 993530973844.dkr.ecr.us-east-1.amazonaws.com/odsextractor:%1%
#@ECHO Any errors Ctrl+C and do something to fix
#pause

#docker push 993530973844.dkr.ecr.us-east-1.amazonaws.com/odsextractor:%1%
#@ECHO Any errors Ctrl+C and do something to fix
#pause