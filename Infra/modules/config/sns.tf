resource "aws_sns_topic" "topic" {
  name = var.topic_name
}

resource "aws_sns_topic_policy" "policy" {
  arn = aws_sns_topic.topic.arn

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
      {
        "Sid": "AWSConfigSNSpublish",
        "Action": "sns:Publish",
        "Effect": "Allow",
        "Resource": "${aws_sns_topic.topic.id}",
        "Principal": {
          "Service": [
            "config.amazonaws.com"
          ]
        }
      }
    ]
}
POLICY
}
