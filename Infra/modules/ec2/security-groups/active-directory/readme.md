# Module - Domain Controller Security Group

- [Module - Domain Controller Security Group](#module---domain-controller-security-group)
  - [Minimum Required Configuration](#minimum-required-configuration)
  - [Inputs and Outputs](#inputs-and-outputs)
    - [Inputs](#inputs)
    - [Outputs](#outputs)

This modules creates 3 security groups for use on Active Directory Domain Controllers and members.

- Domain Controllers - Allows ingress from domain members to AD service ports
- Domain Members - Assign to all domain members to allow base ingress to domain controllers
- Inter DC - Allows full communication between domain controllers

## Minimum Required Configuration

```terraform
module "sgs_active_directory" {
  source   = "../relative/path/to/modules/ec2/security-groups/active-directory"
  vpc_id   = "vpc-XXXXXXXX"
  vpc_cidr = "xxx.xxx.xxx.xxx/xx"
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
| corporate\_domain\_controller\_cidrs | List of CIDR blocks for on-premise Domain Controllers | `list` | `[]` | no |
| tags | Tags to apply to all module resources. | `map` | `{}` | no |
| vpc\_cidr | VPC CIDR | `string` | n/a | yes |
| vpc\_id | VPC ID | `string` | n/a | yes |

### Outputs

| Name | Description |
|------|-------------|
| domain\_controller\_id | Domain Controller Security Group ID |
| domain\_member\_id | Domain member Security Group ID |
| inter\_dc\_id | Inter Domain Controller Security Group ID |
