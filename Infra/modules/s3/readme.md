# Module - S3 Bucket

This module provides a basic S3 deployment with encryption.   Credit goes to the [Terraform S3 module](https://github.com/terraform-aws-modules/terraform-aws-s3-bucket) for the `lifecycle_rule` dynamic implementation.   For advanced use, consider referencing that module.

This module differs from the Terraform maintained module in it's application of encryption and restriction of public access.  More details on each of those are below.

- [Module - S3 Bucket](#module---s3-bucket)
  - [Minimum Required Configuration](#minimum-required-configuration)
  - [ACLs](#acls)
  - [Access Logging](#access-logging)
  - [Encryption and Public Access](#encryption-and-public-access)
    - [Inputs](#inputs)
    - [Outputs](#outputs)

## Example Configurations

Basic bucket:

```terraform
module "s3" {
  source = "../relative/path/to/modules/s3"
  name   = "example-bucket-3891739321"
}
```

Bucket with access logging enabled.  Remember, the target bucket, even if itself, must have the proper ACL set.

```terraform
module "s3" {
  source = "../relative/path/to/modules/s3"
  name   = "example-bucket-3891739321"

  access_log_configuration = [{
      target_bucket = "my-logging-bucket"
      target_prefix = "example-bucket-log"
  }]
}
```

Bucket with lifecycle rule:

```terraform
module "s3" {
  source = "../relative/path/to/modules/s3"
  name   = "example-bucket-3891739321"
  lifecycle_rule = [
    {
      enabled = true
      id      = "example"
      prefix  = "test-data/"
      transition = [
        {
          days          = 30
          storage_class = "STANDARD_IA"
        }
      ]
      expiration = {
        days = 90
      }
    }
  ]
}
```

## ACLs

When selecting bucket ACLs, review the [AWS Documentation](https://docs.aws.amazon.com/AmazonS3/latest/dev/acl-overview.html#canned-acl).  By default buckets have `private` as their ACL.

## Access Logging

When configuring bucket access logging it's important to consider:

- The **target** log bucket must have the `log-delivery-write` ACL set _prior_ to enabling logging on the source bucket.
- The **target** bucket can have SSE/AES256 encryption by default, but _not_ KMS.

For more details review the [AWS Documentation](https://docs.aws.amazon.com/AmazonS3/latest/dev/ServerLogs.html#server-access-logging-overview).

## Encryption and Public Access

It is not possible to deploy an un-encrypted bucket using this module.  In addition to that, public access is restricted by default _regardless of policy_.  This can be overridden via variables if desired.

## Inputs and Outputs

Inputs and outputs are generated with [terraform-docs](https://github.com/segmentio/terraform-docs)

```bash
terraform-docs markdown table . | sed s/##/###/g
```

### Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:-----:|
| access\_log\_configuration | Access Logging Configuration | `list(map(string))` | `[]` | no |
| acl | ACL | `string` | `"private"` | no |
| block\_all\_public\_access | Block all public access | `bool` | `true` | no |
| kms\_key\_arn | KMS Key ARN for S3 encryption.  If empty, SSE/AES256 will be used. | `string` | `""` | no |
| lifecycle\_rule | Map of lifecycle rules | `list(map(string))` | `[]` | no |
| name | Bucket name | `string` | n/a | yes |
| policy | Bucket policy | `string` | `""` | no |
| tags | Tags to apply to all module resources. | `map` | `{}` | no |
| versioning | Enable versioning | `string` | `Disabled` | no |

### Outputs

| Name | Description |
|------|-------------|
| arn | Bucket ARN |
| hosted\_zone\_id | Canonical hosted zone ID of the load balancer.  Used for Route 53 entries. |
| name | Bucket Name/ID |
| region | Bucket region |
