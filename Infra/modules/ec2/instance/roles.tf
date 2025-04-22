resource "aws_iam_role" "ec2_role" {
  count       = var.create_role ? 1 : 0
  name        = "ec2-ssm-${var.name}"
  path        = "/"
  description = "Base EC2 role with access for managed services, such as SSM and Cloudwatch"

  assume_role_policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
      {
        "Effect": "Allow",
        "Principal": {
          "Service": "ec2.amazonaws.com"
        },
        "Action": "sts:AssumeRole"
      }
    ]
}
POLICY

  tags = var.tags
  lifecycle {
    ignore_changes = all
  }
}

resource "aws_iam_instance_profile" "ec2_role" {
  count = var.create_role ? 1 : 0

  name = aws_iam_role.ec2_role[0].name
  role = aws_iam_role.ec2_role[0].name
}

resource "aws_iam_role_policy_attachment" "ec2_role" {
  count = var.create_role ? 1 : 0

  role       = aws_iam_role.ec2_role[0].name
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
}

resource "aws_iam_role_policy_attachment" "ec2_role_buckets" {
  count = var.create_role ? 1 : 0

  role       = aws_iam_role.ec2_role[0].name
  policy_arn = aws_iam_policy.ec2_ssm_buckets[0].arn
}

resource "aws_iam_role_policy_attachment" "ec2_cloudwatch_access" {
  count = var.create_role ? 1 : 0

  role       = aws_iam_role.ec2_role[0].name
  policy_arn = "arn:aws:iam::aws:policy/CloudWatchAgentServerPolicy"
}

resource "aws_iam_policy" "ec2_ssm_buckets" {
  count = var.create_role ? 1 : 0
  name  = "ec2-ssm-s3-${var.name}"

  policy = <<POLICY
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "amazonOwnedS3Buckets",
            "Effect": "Allow",
            "Action": "s3:GetObject",
            "Resource": [
                "arn:aws:s3:::aws-ssm-*/*",
                "arn:aws:s3:::aws-patch-manager-*/*",
                "arn:aws:s3:::aws-patchmanager-macos-*/*",
                "arn:aws:s3:::aws-windows-downloads-*/*",
                "arn:aws:s3:::amazon-ssm-*/*",
                "arn:aws:s3:::amazon-ssm-packages-*/*",
                "arn:aws:s3:::*-birdwatcher-prod/*",
                "arn:aws:s3:::patch-baseline-snapshot-*/*"
            ]
        }

    ]
}
POLICY
  lifecycle {
    ignore_changes = all
  }
}
