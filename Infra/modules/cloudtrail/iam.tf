resource "aws_iam_role" "cloudtrail" {
  name        = "cloudtrail"
  path        = "/"
  description = "Allows AWS CloudTrail write access to CloudWatch logs"

  assume_role_policy = <<POLICY
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "CloudTrailAssumeRole",
      "Effect": "Allow",
      "Principal": {
        "Service": "cloudtrail.amazonaws.com"
      },
      "Action": "sts:AssumeRole"
    }
  ]
}
POLICY
}

# CloudTrail requires CreateLogStream even with a pre-existing log stream
resource "aws_iam_policy" "cloudwatch_log_write" {
  name        = "cloudtrail-cloudwatch-log-write"
  path        = "/"
  description = "Allow CloudTrail role write access to CloudWatch logs"

  policy = <<POLICY
{
        "Version": "2012-10-17",
        "Statement": [
            {
                "Sid": "cloudTrailWriteLogs",
                "Action": [
                    "logs:CreateLogStream",
                    "logs:PutLogEvents"
                ],
                "Resource": [
                  "${aws_cloudwatch_log_group.cloudtrail.arn}:*"
                  ],
                "Effect": "Allow"
            }
        ]
}
POLICY
}

resource "aws_iam_role_policy_attachment" "cloudtrail" {
  role       = aws_iam_role.cloudtrail.name
  policy_arn = aws_iam_policy.cloudwatch_log_write.arn
}
