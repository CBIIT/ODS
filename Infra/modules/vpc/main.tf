data "aws_region" "current" {}

resource "aws_vpc" "vpc" {
  cidr_block           = var.settings["main"]["cidr"]
  enable_dns_support   = true
  enable_dns_hostnames = true
  tags                 = merge(var.tags, { "Name" = var.settings["main"]["name"] })
}

resource "aws_flow_log" "flowlogs" {
  log_destination = aws_cloudwatch_log_group.flowlogs.arn
  iam_role_arn    = aws_iam_role.flowlogs.arn
  vpc_id          = aws_vpc.vpc.id
  traffic_type    = var.flowlog_traffic_type
  tags              = merge(var.tags, { "Name" = var.settings["main"]["name"] })
}

resource "aws_cloudwatch_log_group" "flowlogs" {
  name              = "/aws/flowlogs/${var.settings["main"]["name"]}"
  retention_in_days = var.flowlog_retention
  tags              = merge(var.tags, { "Name" = var.settings["main"]["name"] })
}

resource "aws_internet_gateway" "default" {
  count  = var.no_igw == false ? 1 : 0
  vpc_id = aws_vpc.vpc.id
  tags   = merge(var.tags, { "Name" = var.settings["main"]["name"] })
}

resource "aws_vpc_dhcp_options" "options" {
  domain_name         = var.domain_name != "" ? var.domain_name : null
  domain_name_servers = var.domain_name_servers
  tags                = merge(var.tags, { "Name" = var.settings["main"]["name"] })
}

resource "aws_vpc_dhcp_options_association" "association" {
  vpc_id          = aws_vpc.vpc.id
  dhcp_options_id = aws_vpc_dhcp_options.options.id
}
