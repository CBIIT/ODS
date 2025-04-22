resource "aws_vpc_endpoint" "s3" {
  vpc_id       = aws_vpc.vpc.id
  service_name = "com.amazonaws.${data.aws_region.current.name}.s3"
}

resource "aws_vpc_endpoint" "dynamodb" {
  vpc_id       = aws_vpc.vpc.id
  service_name = "com.amazonaws.${data.aws_region.current.name}.dynamodb"
}



#
# VPC Endpoint - private data subnet route associations
#

resource "aws_vpc_endpoint_route_table_association" "private_s3_data_cidrs" {
  for_each        = toset(var.availability_zones)
  vpc_endpoint_id = aws_vpc_endpoint.s3.id
  route_table_id  = aws_route_table.data[each.key].id
}

resource "aws_vpc_endpoint_route_table_association" "private_dynamodb_data_cidrs" {
  for_each        = toset(var.availability_zones)
  vpc_endpoint_id = aws_vpc_endpoint.dynamodb.id
  route_table_id  = aws_route_table.data[each.key].id
}



#
# VPC Endpoints - Private subnet route associations
#

resource "aws_vpc_endpoint_route_table_association" "private_s3_private_cidrs" {
  for_each        = toset(var.availability_zones)
  vpc_endpoint_id = aws_vpc_endpoint.s3.id
  route_table_id  = aws_route_table.private[each.key].id
}

resource "aws_vpc_endpoint_route_table_association" "private_dynamodb_private_cidrs" {
  for_each        = toset(var.availability_zones)
  vpc_endpoint_id = aws_vpc_endpoint.dynamodb.id
  route_table_id  = aws_route_table.private[each.key].id
}

#
# VPC Endpoints - Public route associations
#

resource "aws_vpc_endpoint_route_table_association" "private_s3_public_cidrs" {
  for_each        = toset(var.availability_zones)
  vpc_endpoint_id = aws_vpc_endpoint.s3.id
  route_table_id  = aws_route_table.public[each.key].id
}

resource "aws_vpc_endpoint_route_table_association" "private_dynamodb_public_cidrs" {
  for_each        = toset(var.availability_zones)
  vpc_endpoint_id = aws_vpc_endpoint.dynamodb.id
  route_table_id  = aws_route_table.public[each.key].id
}

#
# Secrets Manager
#

resource "aws_security_group" "secrets_manager_sg" {
  description = "Security group for Secrets Manager VPC endpoint"
  vpc_id      = aws_vpc.vpc.id

  # Allow all outbound traffic
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = [aws_vpc.vpc.cidr_block]
  }

  # Allow inbound traffic from the subnet
  ingress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}


resource "aws_vpc_endpoint" "secrets_manager" {
  vpc_id = aws_vpc.vpc.id

  service_name = "com.amazonaws.${data.aws_region.current.name}.secretsmanager"
  vpc_endpoint_type = "Interface"
  private_dns_enabled = true
  security_group_ids = [
    aws_security_group.secrets_manager_sg.id,
  ]

  # Specify the subnet IDs where the endpoint will be created
  subnet_ids = [aws_subnet.private[var.availability_zones[0]].id,aws_subnet.private[var.availability_zones[1]].id]

  # Policy to control access to the endpoint
  policy = <<POLICY
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": "*",
      "Action": "*",
      "Resource": "*"
    }
  ]
}
POLICY
}