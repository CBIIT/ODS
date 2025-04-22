# Module - Security Hub

- [Module - Security Hub](#module---security-hub)
  - [Minimum Required Configuration](#minimum-required-configuration)
  - [Standards](#standards)
  - [Inputs and Outputs](#inputs-and-outputs)
    - [Inputs](#inputs)
    - [Outputs](#outputs)

This modules serves as a a way to set up AWS Security Hub.

This module can be used in 1 of 3 ways: standalone, master, or member.

For a standalone account, you can run the module with no arguments.

For a master account (which will also get data from members), you need to pass a map of account numbers and email addresses (root account email) for any of the accounts you want to collect data from.

For a member account, you will run without any arguments and then will need to go in to the console (or CLI) and accept the invitation from the member account under Settings.

## Standards

The CIS standard is enabled by default.  To learn more about Security Hub Standards, please read the information [here](https://docs.aws.amazon.com/securityhub/latest/userguide/securityhub-standards.html).

## Minimum Required Configuration

```terraform
module "security_hub" {
  source = "../relative/path/to/modules/security-hub"
}
```

## Inputs and Outputs

Inputs and outputs are generated with [terraform-docs](https://github.com/segmentio/terraform-docs)

```bash
terraform-docs markdown table . | sed s/##/###/g
```

### Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:--------:|
| invite\_accounts | Map of Account IDs and emails to invite to this master account | `map` | `{}` | no |

### Outputs

No output.
