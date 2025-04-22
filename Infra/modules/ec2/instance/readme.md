# Module - EC2

- [Module - EC2](#module---ec2)
  - [Minimum Required Configuration](#minimum-required-configuration)
  - [Inputs and Outputs](#inputs-and-outputs)
    - [Inputs](#inputs)
    - [Outputs](#outputs)

This modules serves as a baseline for EC2 deployments. Many variables can be overridden to provide consistency across an environment.

This module only provides the EC2 instance. It does **not** create IAM roles or security groups.

## Minimum Required Configuration

EBS encryption is not required but included in this example. Encryption is strongly recommended.

```terraform
module "ec2_example" {
  source = "../relative/path/to/modules/ec2"

  ami                  = "ami-XXXXXXXX"
  key_name             = "XXXXXXXX"

  root_block_device = [{
    encrypted   = true
    kms_key_id  = XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX
    volume_type = "gp3"
    volume_size = 8
  }]

  subnet_id              = "subnet-XXXXXXXX"
  vpc_security_group_ids = [ "sg-XXXXXXXX" ]
  create_role            = true
}
```

## Inputs and Outputs

Inputs and outputs are generated with [terraform-docs](https://github.com/segmentio/terraform-docs)

```bash
terraform-docs markdown table . | sed s/##/###/g
```

### Resources

| Name                                                                                                                                                            | Type        |
| --------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------- |
| [aws_cloudwatch_metric_alarm.instance_status_check_failed](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/cloudwatch_metric_alarm) | resource    |
| [aws_cloudwatch_metric_alarm.system_status_check_failed](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/cloudwatch_metric_alarm)   | resource    |
| [aws_cloudwatch_metric_alarm.cpu_utilization](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/cloudwatch_metric_alarm)              | resource    |
| [aws_cloudwatch_metric_alarm.mem_utilization](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/cloudwatch_metric_alarm)              | resource    |
| [aws_cloudwatch_metric_alarm.disk_utilization](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/cloudwatch_metric_alarm)             | resource    |
| [aws_iam_instance_profile.ec2_role](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/iam_instance_profile)                           | resource    |
| [aws_iam_policy.ec2_ssm_buckets](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/iam_policy)                                        | resource    |
| [aws_iam_role.ec2_role](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/iam_role)                                                   | resource    |
| [aws_iam_role_policy_attachment.ec2_cloudwatch_access](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/iam_role_policy_attachment)  | resource    |
| [aws_iam_role_policy_attachment.ec2_role](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/iam_role_policy_attachment)               | resource    |
| [aws_iam_role_policy_attachment.ec2_role_buckets](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/iam_role_policy_attachment)       | resource    |
| [aws_instance.ec2](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/instance)                                                        | resource    |
| [aws_caller_identity.current](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/data-sources/caller_identity)                                   | data source |
| [aws_region.current](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/data-sources/region)                                                     | data source |

### Inputs

| Name                                                                                                               | Description                                                                                              | Type                | Default       | Required |
| ------------------------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------------------- | ------------------- | ------------- | :------: |
| <a name="input_ami"></a> [ami](#input_ami)                                                                         | Base Amazon Machine Image (AMI)                                                                          | `string`            | n/a           |   yes    |
| <a name="input_associate_public_ip_address"></a> [associate_public_ip_address](#input_associate_public_ip_address) | Associate a public IP address                                                                            | `bool`              | `false`       |    no    |
| <a name="input_alarm_sns_topic_name"></a> [alarm_sns_topic_name](#input_alarm_sns_topic_name)                      | SNS Topic for Alarms                                                                                     | `string`            | `""`          |    no    |
| <a name="input_cpu_utilization"></a> [cpu_utilization](#input_cpu_utilization)                                     | Create Clouwatch Alarm for CPU Utilization?                                                              | `bool`              | `false`       |    no    |
| <a name="input_cpu_threshold"></a> [cpu_threshold](#input_cpu_threshold)                                           | Threshold value for CPU Utilization Alarm                                                                | `number`            | `90`          |    no    |
| <a name="input_cpu_eval_period"></a> [cpu_eval_period](#input_cpu_eval_period)                                     | Evaluation Period for CPU Utilization Alarm                                                              | `number`            | `15`          |    no    |
| <a name="input_cpu_eval_period_length"></a> [cpu_eval_period_length](#input_cpu_eval_period_length)                | Evaluation Period Length for CPU Utilization Alarm                                                       | `number`            | `60`          |    no    |
| <a name="input_mem_utilization"></a> [mem_utilization](#input_mem_utilization)                                     | Create Clouwatch Alarm for MEM Utilization?                                                              | `bool`              | `false`       |    no    |
| <a name="input_mem_threshold"></a> [mem_threshold](#input_mem_threshold)                                           | Threshold value for MEM Utilization Alarm                                                                | `number`            | `90`          |    no    |
| <a name="input_mem_eval_period"></a> [mem_eval_period](#input_mem_eval_period)                                     | Evaluation Period for MEM Utilization Alarm                                                              | `number`            | `15`          |    no    |
| <a name="input_mem_eval_period_length"></a> [mem_eval_period_length](#input_mem_eval_period_length)                | Evaluation Period Length for MEM Utilization Alarm                                                       | `number`            | `60`          |    no    |
| <a name="input_disk_utilization"></a> [disk_utilization](#input_disk_utilization)                                  | Create Clouwatch Alarm for Disk Utilization?                                                             | `bool`              | `false`       |    no    |
| <a name="input_disk_threshold"></a> [disk_threshold](#input_disk_threshold)                                        | Threshold value for Disk Utilization Alarm                                                               | `number`            | `90`          |    no    |
| <a name="input_disk_eval_period"></a> [disk_eval_period](#input_disk_eval_period)                                  | Evaluation Period for Disk Utilization Alarm                                                             | `number`            | `15`          |    no    |
| <a name="input_disk_eval_period_length"></a> [disk_eval_period_length](#input_disk_eval_period_length)             | Evaluation Period Length for Disk Utilization Alarm                                                      | `number`            | `60`          |    no    |
| <a name="input_create_role"></a> [create_role](#input_create_role)                                                 | Create custom IAM role for instance?                                                                     | `bool`              | `false`       |    no    |
| <a name="input_disable_api_termination"></a> [disable_api_termination](#input_disable_api_termination)             | Prevent instances from being accidentally terminated                                                     | `bool`              | `true`        |    no    |
| <a name="input_ebs_block_device"></a> [ebs_block_device](#input_ebs_block_device)                                  | List of maps containing additional EBS volumes                                                           | `list(map(string))` | `[]`          |    no    |
| <a name="input_enable_detailed_monitoring"></a> [enable_detailed_monitoring](#input_enable_detailed_monitoring)    | Detailed monitoring delivers 1 minute instance metrics for an extra cost. Basic monitoring is 5 minutes. | `bool`              | `false`       |    no    |
| <a name="input_iam_instance_profile"></a> [iam_instance_profile](#input_iam_instance_profile)                      | IAM instance profile _Name_ associated with the IAM role to attach to the instance                       | `string`            | `""`          |    no    |
| <a name="input_instance_status_check"></a> [instance_status_check](#input_instance_status_check)                   | Create Cloudwatch Alarm for Instance Status Check?                                                       | `bool`              | `false`       |    no    |
| <a name="input_instance_type"></a> [instance_type](#input_instance_type)                                           | Instance type                                                                                            | `string`            | `"t3a.small"` |    no    |
| <a name="input_key_name"></a> [key_name](#input_key_name)                                                          | Key Pair for instance access (key name, NOT key ID)                                                      | `string`            | n/a           |   yes    |
| <a name="input_name"></a> [name](#input_name)                                                                      | Name of EC2 instance                                                                                     | `string`            | `""`          |    no    |
| <a name="input_private_ip"></a> [private_ip](#input_private_ip)                                                    | Optionally specify a specific private IP address                                                         | `string`            | `""`          |    no    |
| <a name="input_root_block_device"></a> [root_block_device](#input_root_block_device)                               | Root EBS volume definition                                                                               | `list(map(string))` | n/a           |   yes    |
| <a name="input_source_dest_check"></a> [source_dest_check](#input_source_dest_check)                               | Source and destination packet checks. Disable only for NAT or VPN related instances.                     | `bool`              | `true`        |    no    |
| <a name="input_subnet_id"></a> [subnet_id](#input_subnet_id)                                                       | Target subnet for instance deployment                                                                    | `string`            | n/a           |   yes    |
| <a name="input_system_status_check"></a> [system_status_check](#input_system_status_check)                         | Create Cloudwatch Alarm for System Status Check?                                                         | `bool`              | `false`       |    no    |
| <a name="input_tags"></a> [tags](#input_tags)                                                                      | Tags to apply to all module resources.                                                                   | `map`               | `{}`          |    no    |
| <a name="input_user_data"></a> [user_data](#input_user_data)                                                       | User data to launch the instance with. Will be base64 encoded by this module.                            | `string`            | `""`          |    no    |
| <a name="input_volume_tags"></a> [volume_tags](#input_volume_tags)                                                 | Map of tags to add to attached EBS volumes                                                               | `map`               | `{}`          |    no    |
| <a name="input_vpc_security_group_ids"></a> [vpc_security_group_ids](#input_vpc_security_group_ids)                | List of security group IDs to associate with the instance                                                | `list(string)`      | n/a           |   yes    |

### Outputs

| Name                                                                                                                                             | Description              |
| ------------------------------------------------------------------------------------------------------------------------------------------------ | ------------------------ |
| <a name="output_cloudwatch_alarm_arn_instance_fail"></a> [cloudwatch_alarm_arn_instance_fail](#output_cloudwatch_alarm_arn_instance_fail)        | n/a                      |
| <a name="output_cloudwatch_alarm_arn_system_fail"></a> [cloudwatch_alarm_arn_system_fail](#output_cloudwatch_alarm_arn_system_fail)              | n/a                      |
| <a name="output_cloudwatch_alarm_arn_cpu_utilization"></a> [cloudwatch_alarm_id_cpu_utilization](#output_cloudwatch_alarm_id_cpu_utilization)    | n/a                      |
| <a name="output_cloudwatch_alarm_arn_mem_utilization"></a> [cloudwatch_alarm_id_mem_utilization](#output_cloudwatch_alarm_id_mem_utilization)    | n/a                      |
| <a name="output_cloudwatch_alarm_arn_disk_utilization"></a> [cloudwatch_alarm_id_disk_utilization](#output_cloudwatch_alarm_id_disk_utilization) | n/a                      |
| <a name="output_id"></a> [id](#output_id)                                                                                                        | Instance ID              |
| <a name="output_private_ip"></a> [private_ip](#output_private_ip)                                                                                | Private IP Address       |
| <a name="output_public_ip"></a> [public_ip](#output_public_ip)                                                                                   | Public IP Address        |
| <a name="output_security_groups"></a> [security_groups](#output_security_groups)                                                                 | Attached security groups |
| <a name="output_subnet_id"></a> [subnet_id](#output_subnet_id)                                                                                   | Subnet ID                |

| Name                        | Description                                                                                              | Type                | Default       | Required |
| --------------------------- | -------------------------------------------------------------------------------------------------------- | ------------------- | ------------- | :------: |
| ami                         | Base Amazon Machine Image (AMI)                                                                          | `string`            | n/a           |   yes    |
| associate_public_ip_address | Associate a public IP address                                                                            | `bool`              | `false`       |    no    |
| disable_api_termination     | Prevent instances from being accidentally terminated                                                     | `bool`              | `true`        |    no    |
| ebs_block_device            | List of maps containing additional EBS volumes                                                           | `list(map(string))` | `[]`          |    no    |
| enable_detailed_monitoring  | Detailed monitoring delivers 1 minute instance metrics for an extra cost. Basic monitoring is 5 minutes. | `bool`              | `false`       |    no    |
| iam_instance_profile        | IAM instance profile \*Name\* associated with the IAM role to attach to the instance                     | `string`            | `""`          |    no    |
| instance_type               | Instance type                                                                                            | `string`            | `"t3a.small"` |    no    |
| key_name                    | Key Pair for instance access (key name, NOT key ID)                                                      | `string`            | n/a           |   yes    |
| private_ip                  | Optionally specify a specific private IP address                                                         | `string`            | `""`          |    no    |
| root_block_device           | Root EBS volume definition                                                                               | `list(map(string))` | n/a           |   yes    |
| source_dest_check           | Source and destination packet checks. Disable only for NAT or VPN related instances.                     | `bool`              | `true`        |    no    |
| subnet_id                   | Target subnet for instance deployment                                                                    | `string`            | n/a           |   yes    |
| tags                        | Tags to apply to all module resources.                                                                   | `map`               | `{}`          |    no    |
| user_data                   | User data to launch the instance with. Will be base64 encoded by this module.                            | `string`            | `""`          |    no    |
| volume_tags                 | Map of tags to add to attached EBS volumes                                                               | `map`               | `{}`          |    no    |
| vpc_security_group_ids      | List of security group IDs to associate with the instance                                                | `list(string)`      | n/a           |   yes    |
