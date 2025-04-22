resource "aws_ec2_transit_gateway" "tgw" {
  auto_accept_shared_attachments = var.auto_accept_shared_attachments
  tags                           = merge(var.tags, { "Name" = var.name })
}

resource "aws_ram_resource_share" "tgw" {
  allow_external_principals = var.allow_external_principals
  name                      = var.name
  tags                      = var.tags
}

resource "aws_ram_resource_association" "tgw" {
  resource_arn       = aws_ec2_transit_gateway.tgw.arn
  resource_share_arn = aws_ram_resource_share.tgw.id
}

resource "aws_ram_principal_association" "tgw" {
  for_each           = toset(var.target_share_accounts)
  principal          = each.key
  resource_share_arn = aws_ram_resource_share.tgw.id
}
