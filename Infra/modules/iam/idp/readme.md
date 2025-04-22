# Module - idp

- [Module - idp](#module---idp)
  - [Common SAML Providers](#common-saml-providers)
  - [Minimum Required Configuration](#minimum-required-configuration)
  - [Providers, Inputs and Outputs](#providers-inputs-and-outputs)
    - [Providers](#providers)
    - [Inputs](#inputs)
    - [Outputs](#outputs)

This module creates an AWS IAM Identity Provider for use with a SAML provider.

## Common SAML Providers

- [Okta](https://help.okta.com/en/prod/Content/Topics/DeploymentGuides/AWS/aws-deployment.htm)
- [GSuite](https://aws.amazon.com/blogs/security/how-to-set-up-federated-single-sign-on-to-aws-using-google-apps/)
- [AzureAD](https://docs.microsoft.com/en-us/azure/active-directory/saas-apps/amazon-web-service-tutorial)

If you're creating the IAM User, you'll need to manually generate the Access Key after applying.

## Minimum Required Configuration

```terraform
module "idp" {
  source = "path/to/modules/iam/idp-auth"

  create_user             = true
  provider_name           = "azuread"
  provider_metadata_file  = "saml-metadata.xml"
}

resource "aws_iam_role" "admins" {
  name = "admins"

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "",
      "Effect": "Allow",
      "Principal": {
        "Federated": "${module.idp.provider_arn}"
      },
      "Action": "sts:AssumeRoleWithSAML",
      "Condition": {
        "StringEquals": {
          "SAML:aud": "https://signin.aws.amazon.com/saml"
        }
      }
    }
  ]
}
EOF

depends_on = [ module.idp ]
}

resource "aws_iam_policy" "admins" {
  name        = "admins"
  description = "grants role access to assume specific roles"

  policy = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Action": "sts:AssumeRole",
            "Resource": [
                "arn:aws:iam::${var.aws_accounts["security"].account_id}:role/admins",
                "arn:aws:iam::${var.aws_accounts["logging"].account_id}:role/admins",
                "arn:aws:iam::${var.aws_accounts["staging"].account_id}:role/admins",
                "arn:aws:iam::${var.aws_accounts["production"].account_id}:role/admins",
                "arn:aws:iam::${var.aws_accounts["development"].account_id}:role/admins"
            ],
            "Effect": "Allow"
        }
    ]
}
EOF
}

resource "aws_iam_policy_attachment" "admins_assume" {
  name = "admins"

  roles      = [ aws_iam_role.admins.name ]
  policy_arn = aws_iam_policy.admins.arn
}

resource "aws_iam_role_policy_attachment" "admins_administrator" {
  role       = aws_iam_role.admins.name
  policy_arn = "arn:aws:iam::aws:policy/AdministratorAccess"
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
| create\_user | Flag to create user for listing roles (not needed for gsuite) | `bool` | n/a | yes |
| provider\_metadata\_file | n/a | `string` | n/a | yes |
| provider\_name | The name of your provider, which could be case sensitive (okta, azuread, gsuite, etc) | `string` | n/a | yes |

### Outputs

| Name | Description |
|------|-------------|
| provider\_arn | The ARN of the IAM Identity Provider being created |
