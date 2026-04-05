#!/bin/bash

# IssueGPT Kubernetes 清理腳本

set -e

echo "🧹 IssueGPT K8s 清理"
echo ""
echo "⚠️  此操作將刪除所有 IssueGPT 資源"
echo ""
read -p "確認刪除？(y/n) " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "✅ 已取消"
    exit 0
fi

echo ""
echo "刪除 IssueGPT 命名空間..."
kubectl delete namespace issuegpt --ignore-not-found=true

echo "清理 MSSQL 數據..."
rm -rf /tmp/mssql-data 2>/dev/null || true

echo ""
echo "✅ 清理完成"
echo ""
echo "💡 若要刪除本地 Docker 鏡像："
echo "docker rmi issuegpt-api:latest issuegpt-frontend:latest"
