resource "aws_iam_user" "idpuser" {
  count = var.create_user ? 1 : 0

  name = var.provider_name
  path = "/"
}

resource "aws_iam_policy" "idp_list_roles" {
  count = var.create_user ? 1 : 0

  name = "idp_list_roles"

  policy = <<EOF
{
  "Version":"2012-10-17",
  "Statement":[
     {
        "Effect":"Allow",
        "Action":[
          "iam:ListRoles",
          "iam:ListAccountAliases"
        ],
        "Resource":"*"
     }
  ]
}
EOF
}

resource "aws_iam_user_policy_attachment" "idp_list_roles" {
  count = var.create_user ? 1 : 0

  user       = aws_iam_user.idpuser[0].name
  policy_arn = aws_iam_policy.idp_list_roles[0].arn
}

resource "aws_iam_saml_provider" "idp" {
  name                   = var.provider_name
  saml_metadata_document = file(var.provider_metadata_file)

  lifecycle {
    ignore_changes = [saml_metadata_document]
  }
}
