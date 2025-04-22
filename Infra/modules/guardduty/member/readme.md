# Module - GuardDuty Member

- [Module - GuardDuty Member](#module---guardduty-member)
  - [Minimum Required Configuration](#minimum-required-configuration)
  - [Inputs and Outputs](#inputs-and-outputs)
    - [Inputs](#inputs)
    - [Outputs](#outputs)

This modules creates a [Guarduty Member Account](https://docs.aws.amazon.com/guardduty/latest/ug/guardduty_accounts.html#guardduty_member) configuration.  Invitations should be sent from the master account prior to executing this module.  When run, this module will create a detector and accept the pending invite from the master account.

## Minimum Required Configuration

```terraform
module "guardduty_member" {
  source = "../relative/path/to/modules/guardduty/member"

  master_aws_account_id = "999999999999"
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
| enable | Enabled GuardDuty | `bool` | `true` | no |
| master\_aws\_account\_id | Account ID of master account | `string` | n/a | yes |

### Outputs

| Name | Description |
|------|-------------|
| member\_aws\_account\_id | n/a |
| member\_detector\_id | n/a |
