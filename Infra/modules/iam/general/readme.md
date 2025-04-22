# Module - IAM

- [Module - IAM](#module---iam)
  - [Password Policy](#password-policy)
  - [EC2 IAM Role](#ec2-iam-role)
  - [Standard Roles](#standard-roles)
  - [Service Linked Roles](#service-linked-roles)
  - [Minimum Required Configuration](#minimum-required-configuration)
  - [Providers, Inputs and Outputs](#providers-inputs-and-outputs)
    - [Providers](#providers)
    - [Inputs](#inputs)
    - [Outputs](#outputs)

This module provides a location for IAM resources that may be common across all environments.

Roles in this module are split into standard IAM roles and [AWS Service Linked Roles](https://docs.aws.amazon.com/IAM/latest/UserGuide/using-service-linked-roles.html).   Service linked roles are created via Terraform but managed by AWS.

## Minimum Required Configuration

```terraform
module "iam_general" {
  source                    = "../relative/path/to/modules/iam/general"
}
```

## Password Policy

A password policy is created by default, tracking [CIS AWS Foundations Benchmark controls](https://docs.aws.amazon.com/securityhub/latest/userguide/securityhub-cis-controls.html#securityhub-cis-controls-1.5).

## EC2 IAM Role

The `ec2-ssm` role is configured to grant an ec2 instance the minimum required permissions for SSM, access to SSM related S3 buckets, and access to publish logs and metrics to Cloudwatch.  Configuring the Cloudwatch agent, Cloudwatch Log Groups, and SSM Patch Manager are done separately from this module.

## Standard Roles

Service roles are assumed by an AWS service.  Standard roles are assumed by users, roles, or instances.  None of these roles are deployed by default. See module inputs for more details.

| Resource        |  Type                | Description                                                               |
|:---------------:|----------------------|---------------------------------------------------------------------------|
| aws_backup      | Service Role         | Allows AWS Backup to perform backup operations for all the AWS services that it supports. |
| batch           | Service Role         | Allows Batch service operations against EC2 services.                     |
| batch_instance  | Role         | Used by Batch EC2 instances for job execution                             |
| cloudformation  | Service Role         | Allows CloudFormation operations supporting [Lambda SAM deployments](https://docs.aws.amazon.com/lambda/latest/dg/build-pipeline.html#with-pipeline-create-cfn-role).       |
| codebuild       | Service Role         | Allows CodeBuild operations against services like ECR, S3, etc.           |
| codepipeline    | Service Role         | Allows CodePipeline operations against services like ECR, CodeBuild, etc. |
| dlm             | Service Role         | Allows Amazon Data Lifecycle Manager to perform the actions required by snapshot and cross-account snapshot copy policies on your behalf. |
| ec2_ssm         | Role                 | Can be associated with EC2 instances to allow [AWS Systems Manager](https://docs.aws.amazon.com/systems-manager/latest/userguide/session-manager.html) ingress, [Patch Manager](https://docs.aws.amazon.com/systems-manager/latest/userguide/systems-manager-patch.html), and writing custom logs and metrics to [Cloudwatch](https://docs.aws.amazon.com/AmazonCloudWatch/latest/monitoring/Install-CloudWatch-Agent.html) |
| spot_fleet      | Service Role         | Allows Spot Fleet operations                                              |

## Service Linked Roles

Service linked roles _are_ created by default.  AWS fully manages the policies attached to these roles post creation.

| Resource        |  Description                                                       |
|:---------------:|------------------------------------------------------------------- |
| AWS autoscaling | Allows the AWS Autoscaling Service to perform EC2 scaling actions. |
| AWS RDS         | Allows the RDS Service access to resources like CloudWatch.        |
| AWS spot        | Allows AWS spot service to perform EC2 actions.                    |

## Minimum Required Configuration

```terraform
module "iam" {
  source = "path/to/modules/iam/general"
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
|------|-------------|------|---------|:--------:|
| password\_policy\_allow\_users\_to\_change | Allow users to change their own password | `bool` | `true` | no |
| password\_policy\_enabled | Create the password policy | `bool` | `true` | no |
| password\_policy\_max\_age | Passwords are expired after this many days | `number` | `90` | no |
| password\_policy\_minimum\_length | Minimum length | `number` | `14` | no |
| password\_policy\_require\_lowercase\_characters | Passwords must contain a lowercase character | `bool` | `true` | no |
| password\_policy\_require\_numbers | Passwords must contain a number | `bool` | `true` | no |
| password\_policy\_require\_symbols | Password must contain a symbol | `bool` | `true` | no |
| password\_policy\_require\_uppercase\_characters | Passwords must contain an uppercase character | `bool` | `true` | no |
| password\_policy\_reuse\_prevention | Prevent using this number of previous passwords | `number` | `3` | no |
| role\_batch | Create batch service role and batch EC2 instance role | `bool` | `false` | no |
| role\_cloudformation | Create cloudformation service role | `bool` | `false` | no |
| role\_codebuild | Create codebuild service role | `bool` | `false` | no |
| role\_codebuild\_ecr\_push | Allow the codebuild service role to push to ECR repositories | `bool` | `false` | no |
| role\_codepipeline | Create codepipeline service role | `bool` | `false` | no |
| role\_ec2\_ssm | Create role with SSM core policy attached | `bool` | `true` | no |
| role\_spot\_fleet | Create spot fleet service role | `bool` | `false` | no |
| tags | Tags to apply to all module resources. | `map` | `{}` | no |

### Outputs

| Name | Description |
|------|-------------|
| profile\_batch\_instance\_arn | ARN of the batch EC2 instance profile |
| profile\_ec2\_ssm\_arn | ARN of the ec2-ssm instance profile |
| role\_batch\_instance\_arn | ARN of the batch EC2 instance role |
| role\_ec2\_ssm\_name | Name of the ec2-ssm role |
| service\_linked\_role\_autoscaling\_arn | ARN of the autoscaling service linked role |
| service\_linked\_role\_rds\_arn | ARN of the RDS service linked role |
| service\_linked\_role\_spot\_arn | ARN of the Spot service linked role |
| service\_role\_batch\_arn | ARN of the batch service role |
| service\_role\_cloudformation\_arn | ARN of the Cloudformation service role |
| service\_role\_cloudformation\_name | Name of the Cloudformation service role |
| service\_role\_codebuild\_arn | ARN of the CodeBuild service role |
| service\_role\_codebuild\_name | Name of the CodeBuild service role |
| service\_role\_codepipeline\_arn | ARN of the CodePipeline service role |
| service\_role\_codepipeline\_name | Name of the CodePipeline service role |
| service\_role\_spot\_fleet\_arn | ARN of the spot fleet service role |
| service\_role\_backup_\_arn | ARN of the AWS Backup service role |