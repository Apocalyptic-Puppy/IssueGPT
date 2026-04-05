#!/bin/bash

# IssueGPT Kubernetes 一鍵部署腳本
# 用於 Mac 本地開發環境

set -e  # 遇到錯誤停止執行

echo "================================"
echo "🚀 IssueGPT K8s 部署開始"
echo "================================"
echo ""

# 顏色定義
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Step 1: 檢查前置需求
echo -e "${YELLOW}Step 1: 檢查前置需求...${NC}"
if ! command -v kubectl &> /dev/null; then
    echo -e "${RED}❌ kubectl 未安裝${NC}"
    exit 1
fi
echo -e "${GREEN}✅ kubectl 已安裝${NC}"

if ! command -v docker &> /dev/null; then
    echo -e "${RED}❌ Docker 未安裝${NC}"
    exit 1
fi
echo -e "${GREEN}✅ Docker 已安裝${NC}"

# Step 2: 構建 Docker 鏡像
echo ""
echo -e "${YELLOW}Step 2: 構建 Docker 鏡像...${NC}"
echo "構建 API 鏡像..."
docker build -f Dockerfile.api -t issuegpt-api:latest . --quiet
echo -e "${GREEN}✅ API 鏡像構建完成${NC}"

echo "構建 Frontend 鏡像..."
docker build -f Dockerfile.frontend -t issuegpt-frontend:latest . --quiet
echo -e "${GREEN}✅ Frontend 鏡像構建完成${NC}"

# Step 3: 創建 StorageClass
echo ""
echo -e "${YELLOW}Step 3: 創建 StorageClass...${NC}"
cat << 'EOF' | kubectl apply -f - -q
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: local-storage
provisioner: kubernetes.io/no-provisioner
volumeBindingMode: WaitForFirstConsumer
EOF
echo -e "${GREEN}✅ StorageClass 創建完成${NC}"

# Step 4: 部署命名空間與配置
echo ""
echo -e "${YELLOW}Step 4: 部署命名空間與配置...${NC}"
kubectl apply -f k8s/namespace.yaml -q
kubectl apply -f k8s/configmap.yaml -q
kubectl apply -f k8s/secret.yaml -q
echo -e "${GREEN}✅ 命名空間與配置部署完成${NC}"

# Step 5: 部署 MSSQL
echo ""
echo -e "${YELLOW}Step 5: 部署 MSSQL...${NC}"
kubectl apply -f k8s/mssql-storage.yaml -q
kubectl apply -f k8s/mssql-statefulset.yaml -q
echo "等待 MSSQL 啟動 (最多 5 分鐘)..."
kubectl wait --for=condition=ready pod -l app=mssql -n issuegpt --timeout=300s 2>/dev/null
echo -e "${GREEN}✅ MSSQL 部署完成並就緒${NC}"

# Step 6: 部署 API
echo ""
echo -e "${YELLOW}Step 6: 部署 API...${NC}"
kubectl apply -f k8s/api-deployment.yaml -q
echo "等待 API 啟動..."
kubectl wait --for=condition=ready pod -l app=issuegpt-api -n issuegpt --timeout=300s 2>/dev/null
echo -e "${GREEN}✅ API 部署完成並就緒${NC}"

# Step 7: 部署 Frontend
echo ""
echo -e "${YELLOW}Step 7: 部署 Frontend...${NC}"
kubectl apply -f k8s/frontend-deployment.yaml -q
echo "等待 Frontend 啟動..."
kubectl wait --for=condition=ready pod -l app=issuegpt-frontend -n issuegpt --timeout=300s 2>/dev/null
echo -e "${GREEN}✅ Frontend 部署完成並就緒${NC}"

# 完成
echo ""
echo "================================"
echo -e "${GREEN}✅ 部署成功！${NC}"
echo "================================"
echo ""
echo -e "${YELLOW}📍 在新的終端窗口中執行下列命令：${NC}"
echo ""
echo "前端 (http://localhost:3000)："
echo -e "${GREEN}kubectl port-forward -n issuegpt svc/issuegpt-frontend 3000:80${NC}"
echo ""
echo "API (http://localhost:5000)："
echo -e "${GREEN}kubectl port-forward -n issuegpt svc/issuegpt-api 5000:80${NC}"
echo ""
echo "MSSQL (localhost:1433 - 可選)："
echo -e "${GREEN}kubectl port-forward -n issuegpt svc/mssql 1433:1433${NC}"
echo ""
echo -e "${YELLOW}📊 查看狀態：${NC}"
echo -e "${GREEN}kubectl get all -n issuegpt${NC}"
echo ""
echo -e "${YELLOW}🔍 檢查 Pod 日誌：${NC}"
echo -e "${GREEN}kubectl logs -f deployment/issuegpt-api -n issuegpt${NC}"
echo ""
