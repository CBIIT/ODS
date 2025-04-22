# Module - KMS

- [Module - KMS](#module---kms)
  - [Minimum Required Configuration](#minimum-required-configuration)
  - [Providers, Inputs and Outputs](#providers-inputs-and-outputs)
    - [Providers](#providers)
    - [Inputs](#inputs)
    - [Outputs](#outputs)

This module creates customer managed KMS keys with optional yearly rotation. Keys are created on a per AWS service basis. Isolating keys in this fashion allows key sharing across accounts without exposing, for example, a key that encrypts both EBS and RDS. Optionally, This module can enable EBS encryption for new volumes by default.

There are cases where both standard IAM roles and [AWS IAM service-linked roles](https://docs.aws.amazon.com/IAM/latest/UserGuide/using-service-linked-roles.html) may require access to a particular KMS key. Using the EBS key as an example, the autoscaling _service-linked_ role and batch service role require access for proper Batch functionality. This access is adjusted via the module input variables.

## Minimum Required Configuration

```terraform
module "kms" {
  source                    = "../relative/path/to/modules/kms"
  ebs_encryption_by_default = false
}
```

## Providers, Inputs and Outputs

Inputs and outputs are generated with [terraform-docs](https://github.com/segmentio/terraform-docs)

```bash
terraform-docs markdown table . | sed s/##/###/g
```

### Providers

| Name | Version |
| ---- | ------- |
| aws  | n/a     |

### Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:--------:|
| backup | Enable customer managed KMS key for use with AWS Backup | `bool` | `false` | no |
| dynamodb | Enable customer managed KMS key for use with DynamoDB Tables | `bool` | `false` | no |
| ebs | Enable customer managed KMS key for use with EBS volumes | `bool` | `true` | no |
| ebs\_encryption\_by\_default | Enable EBS volume encryption by default using customer managed KMS key | `bool` | `false` | no |
| efs | Enable customer managed KMS key for use with EFS | `bool` | `false` | no |
| elasticache | Enable customer managed KMS key for ElastiCache | `bool` | `false` | no |
| kms\_key\_deletion\_window\_in\_days | Number of days before a key is removed after being marked for deletion.  7-30 days. | `number` | `30` | no |
| kms\_key\_rotation | AWS managed rotation of KMS key.  Occurs automatically each year. | `bool` | `true` | no |
| rds | Enable customer managed KMS key for RDS | `bool` | `false` | no |
| s3 | Enable customer managed KMS key for use with s3 buckets | `bool` | `false` | no |
| secretsmanager | Enable customer managed KMS key for SecretsManager | `bool` | `false` | no |
| service\_linked\_role\_spot\_arn | Spot service linked role ARN.  If supplied, proper access to the EBS key will be granted. | `string` | `""` | no |
| service\_role\_batch\_arn | Batch service role ARN.  If supplied, proper access to the EBS key will be granted. | `string` | `""` | no |
| service\_role\_spot\_fleet\_arn | Spot Fleet service role ARN.  If supplied, proper access to the EBS key will be granted. | `string` | `""` | no |
| sns | Enable customer managed KMS key for SNS | `bool` | `false` | no |
| sqs | Enable customer managed KMS key for SQS | `bool` | `false` | no |
| ssm | Enable customer managed KMS key for SSM | `bool` | `false` | no |
| tags | Tags to apply to all module resources. | `map(any)` | `{}` | no |
| workspaces | Enable customer managed KMS key for WorkSpaces | `bool` | `false` | no |

### Outputs

| Name | Description |
|------|-------------|
| backup\_key\_arn | ARN of the backup key |
| backup\_key\_id | ID of the backup key |
| dynamodb\_key\_arn | ARN of the Dynamodb key |
| dynamodb\_key\_id | ID of the Dynamodb key |
| ebs\_key\_arn | ARN of the EBS key |
| ebs\_key\_id | ID of the EBS key |
| efs\_key\_arn | ARN of the efs key |
| efs\_key\_id | ID of the efs key |
| elasticache\_key\_arn | ARN of the ElastiCache key |
| elasticache\_key\_id | ID of the ElastiCache key |
| rds\_key\_arn | ARN of the RDS key |
| rds\_key\_id | ID of the RDS key |
| s3\_key\_arn | ARN of the S3 key |
| s3\_key\_id | ID of the S3 key |
| secretsmanager\_key\_arn | ARN of the SecretsManager key |
| secretsmanager\_key\_id | ID of the SecretsManager key |
| sns\_key\_arn | ARN of the SNS key |
| sns\_key\_id | ID of the SNS key |
| sqs\_key\_arn | ARN of the sqs key |
| sqs\_key\_id | ID of the sqs key |
| ssm\_key\_arn | ARN of the SSM key |
| ssm\_key\_id | ID of the SSM key |
| workspaces\_key\_arn | ARN of the WorkSpaces key |
| workspaces\_key\_id | ID of the WorkSpaces key |