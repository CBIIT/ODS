# Module - CloudCheckr

- [Module - CloudCheckr](#module---cloudcheckr)
  - [Inputs and Outputs](#inputs-and-outputs)
    - [Inputs](#inputs)
    - [Outputs](#outputs)

This module creates an IAM role with permissions for Privo's Cost Optimization service.  The `external_id` variable will be supplied by Privo after CloudCheckr account creation.  The `role_arn` output should be entered into the CloudCheckr UI after this module has been applied.

This module also contains additional policies to monitor CloudTrail activity or Cost and Usage Reports (CUR) when deployed in an AWS Organization/Payer account.  Enter values in the appropriate optional inputs to enable those features.

IAM policies in this module are based on the CloudCheckr IAM policies [documented here](https://success.cloudcheckr.com/article/hopg5xe7ps-create-least-privilege-policies).

```terraform
module "cloudcheckr" {
  source      = "../relative/path/to/modules/iam/cloudcheckr"
  external_id = "CC-XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
}
```

## Inputs and Outputs

Inputs and outputs are generated with [terraform-docs](https://github.com/segmentio/terraform-docs)

```bash
terraform-docs markdown table . | sed s/##/###/g
```

### Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:-----:|
| external\_id | Role external ID.  Provided after CloudCheckr account creation. Example: CC-XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX | `string` | n/a | yes |
| s3\_cloudtrail\_bucket | If empty, cloudtrail-<ACCOUNTID> will be substituted. | `string` | `""` | no |
| s3\_cost\_and\_usage\_bucket | If this account is your Organization payer account, enter the name of the CUR bucket. | `string` | `""` | no |

### Outputs

| Name | Description |
|------|-------------|
| role\_arn | ARN of CloudCheckr role |
| role\_name | Name of the CloudCheckr role |
