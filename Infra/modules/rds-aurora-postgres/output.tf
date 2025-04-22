output "cluster_endpoint" {
  description = "The cluster endpoint"
  value       = aws_rds_cluster.this.endpoint
}

output "cluster_reader_endpoint" {
  description = "The cluster reader endpoint"
  value       = aws_rds_cluster.this.reader_endpoint
}

output "cluster_id" {
  description = "The RDS cluster ID"
  value       = aws_rds_cluster.this.id
}

output "security_group_id" {
  description = "The security group ID"
  value       = aws_security_group.this.id
}

output "writer_instance_id" {
  description = "The ID of the writer instance"
  value       = aws_rds_cluster_instance.writer.id
}

output "reader_instance_id" {
  description = "The ID of the reader instance"
  value       = aws_rds_cluster_instance.reader.id
}