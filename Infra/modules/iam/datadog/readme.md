# Module - Datadog

- [Module - Datadog](#module---datadog)
  - [Providers, Inputs and Outputs](#providers-inputs-and-outputs)
    - [Providers](#providers)
    - [Inputs](#inputs)
    - [Outputs](#outputs)

This module creates an IAM role with permissions for datadog's AWS integration.  The `external_id` variable will be supplied by the datadog integration.  [Privo's datadog repository](https://github.com/privoit/datadog/blob/master/docs/aws_integration.md) has additional instructions.  

IAM policies in this module are based on the Datadog IAM policies [documented here](https://docs.datadoghq.com/integrations/amazon_web_services/?tab=manual#all-permissions).

```terraform
module "datadog" {
  source = "../../modules/iam/datadog"

  datadog_aws_integration_external_id = "e652b3e4701d420Prr50xfffdd350cee"
}
```

## Providers, Inputs and Outputs

Inputs and outputs are generated with [terraform-docs](https://github.com/segmentio/terraform-docs)

```bash
terraform-docs markdown table . | sed s/##/###/g
```

### Providers

| Name | Version |
|------|---------|
| aws | n/a |

### Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:-----:|
| datadog\_aws\_integration\_external\_id | datadog external id | `any` | n/a | yes |

### Outputs

| Name | Description |
|------|-------------|
| name | n/a |
