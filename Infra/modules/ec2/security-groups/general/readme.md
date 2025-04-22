# Module - General Security Groups

- [Module - Domain Controller Security Group](#module---domain-controller-security-group)
  - [Minimum Required Configuration](#minimum-required-configuration)
  - [Inputs and Outputs](#inputs-and-outputs)
    - [Inputs](#inputs)
    - [Outputs](#outputs)

This modules creates a security groups that may be helpful across a family of accounts.

| Resource        | Description |
|:---------------:|---------------------------------------------------------|
| corp_all | Allow all traffic to and from the corporate network CIDR (VPN) |
| icmp_all | Allows ICMP to and from ALL                                    |
| vpc_all  | Allow all traffic to and from the VPC CIDR                     |

## Minimum Required Configuration

```terraform
module "sgs_general" {
  source   = "../relative/path/to/modules/ec2/security-groups/general"
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
| corporate\_cidr | CIDR block for on-premise connectivity | `string` | `""` | no |
| tags | Tags to apply to all module resources. | `map` | `{}` | no |
| vpc\_cidr | VPC CIDR | `string` | n/a | yes |
| vpc\_id | VPC ID | `string` | n/a | yes |

### Outputs

| Name | Description |
|------|-------------|
| corp\_vpn\_id | Corporate VPN security group ID |
| icmp\_id | ICMP from all security group ID |
| vpc\_id | Local VPC security group ID |
