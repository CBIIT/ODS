# Module - GuardDuty Master

- [Module - GuardDuty Master](#module---guardduty-master)
  - [Minimum Required Configuration](#minimum-required-configuration)
  - [Inputs and Outputs](#inputs-and-outputs)
    - [Inputs](#inputs)
    - [Outputs](#outputs)

This modules creates a [Guarduty Master Account](https://docs.aws.amazon.com/guardduty/latest/ug/guardduty_accounts.html#guardduty_master) configuration.  If desired, member accounts can be invited by supplying a map of AWS Account IDs with associated root account email addresses.  When adding member accounts, send invitations via this module before running the member module in the sub accounts.   This will ensure proper additions.

## Minimum Required Configuration

```terraform
module "guardduty_master" {
  source = "../relative/path/to/modules/guardduty/master"

  invite_accounts = {
    "999999999999" = "aws_root_account_email@company.com"
  }
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
| enable | Enable GuardDuty | `bool` | `true` | no |
| finding\_publishing\_frequency | Notification frequency - FIFTEEN\_MINUTES, ONE\_HOUR, or SIX\_HOURS | `string` | `"SIX\_HOURS"` | no |
| invite\_accounts | Map of Account IDs and emails to invite to this master account | `map` | `{}` | no |
| invite\_message | Message to send in invite | `string` | `"Please accept this GuardDuty invitation."` | no |

### Outputs

| Name | Description |
|------|-------------|
| master\_aws\_account\_id | n/a |
| master\_detector\_id | n/a |
