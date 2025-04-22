resource "aws_securityhub_account" "main" {
}

resource "aws_securityhub_standards_subscription" "cis" {
  depends_on    = [aws_securityhub_account.main]
  standards_arn = "arn:aws:securityhub:::ruleset/cis-aws-foundations-benchmark/v/1.2.0"
}

resource "aws_securityhub_member" "member" {
  depends_on = [aws_securityhub_account.main]

  for_each = var.invite_accounts

  account_id = each.key
  email      = each.value
  invite     = true
}

