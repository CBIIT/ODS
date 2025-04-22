resource "aws_kms_key" "cloudtrail" {
  description             = "CloudTrail"
  deletion_window_in_days = var.kms_key_deletion_window_in_days
  enable_key_rotation     = var.kms_key_rotation

  policy = <<POLICY
  {
    "Version": "2012-10-17",
    "Id": "Key policy created by CloudTrail",
    "Statement": [
      {
        "Sid": "Enable IAM User Permissions",
        "Effect": "Allow",
        "Principal": {
          "AWS": [
            "arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"
          ]
        },
        "Action": "kms:*",
        "Resource": "*"
      },
      {
        "Sid": "Allow CloudTrail to encrypt logs",
        "Effect": "Allow",
        "Principal": {
          "Service": "cloudtrail.amazonaws.com"
        },
        "Action": "kms:GenerateDataKey*",
        "Resource": "*",
        "Condition": {
          "StringLike": {
            "kms:EncryptionContext:aws:cloudtrail:arn": "arn:aws:cloudtrail:*:${data.aws_caller_identity.current.account_id}:trail/*"
          }
        }
      },
      {
        "Sid": "Allow CloudTrail to describe key",
        "Effect": "Allow",
        "Principal": {
          "Service": "cloudtrail.amazonaws.com"
        },
        "Action": "kms:DescribeKey",
        "Resource": "*"
      },
      {
        "Sid": "Allow principals in the account to decrypt log files",
        "Effect": "Allow",
        "Principal": {
          "AWS": "*"
        },
        "Action": [
          "kms:Decrypt",
          "kms:ReEncryptFrom"
        ],
        "Resource": "*",
        "Condition": {
          "StringEquals": {
            "kms:CallerAccount": "${data.aws_caller_identity.current.account_id}"
          },
          "StringLike": {
            "kms:EncryptionContext:aws:cloudtrail:arn": "arn:aws:cloudtrail:*:${data.aws_caller_identity.current.account_id}:trail/*"
          }
        }
      },
      {
        "Sid": "Allow alias creation during setup",
        "Effect": "Allow",
        "Principal": {
          "AWS": "*"
        },
        "Action": "kms:CreateAlias",
        "Resource": "*",
        "Condition": {
          "StringEquals": {
            "kms:CallerAccount": "${data.aws_caller_identity.current.account_id}",
            "kms:ViaService": "ec2.${data.aws_region.current.name}.amazonaws.com"
          }
        }
      },
      {
        "Sid": "Enable cross account log decryption",
        "Effect": "Allow",
        "Principal": {
          "AWS": "*"
        },
        "Action": [
          "kms:Decrypt",
          "kms:ReEncryptFrom"
        ],
        "Resource": "*",
        "Condition": {
          "StringEquals": {
            "kms:CallerAccount": "${data.aws_caller_identity.current.account_id}"
          },
          "StringLike": {
            "kms:EncryptionContext:aws:cloudtrail:arn": "arn:aws:cloudtrail:*:${data.aws_caller_identity.current.account_id}:trail/*"
          }
        }
      }
    ]
  }
POLICY
  tags   = merge(var.tags, { "Name" = var.name })
}

resource "aws_kms_alias" "cloudtrail" {
  name          = "alias/cloudtrail"
  target_key_id = aws_kms_key.cloudtrail.key_id
}