# 🔄 HƯỚNG DẪN: CHUYỂN ĐỔI PLAYER HIỆN TẠI THÀNH PLAYERBODY

## 🎯 Mục tiêu
Giữ nguyên Player GameObject hiện tại đang hoạt động, chỉ thêm components để hỗ trợ Possession System.

**❌ KHÔNG CẦN:**
- Đổi Tag "Player" → "Possessable"
- Xóa script PlayerController cũ (giữ lại để backup)
- Tạo Player mới từ đầu

**✅ CHỈ CẦN:**
- Thêm 2 components vào Player hiện tại
- Disable script cũ tạm thời
- Test

---

## 📋 BƯỚC 1: Backup Player hiện tại

1. **Chọn Player GameObject trong Hierarchy**
2. **Right Click → Duplicate** (hoặc Ctrl+D)
3. Đổi tên thành `Player_Backup`
4. **Disable** GameObject backup (uncheck ở Inspector)

→ Bây giờ bạn có bản backup an toàn! ✅

---

## 📋 BƯỚC 2: Thêm Components vào Player

### 2.1. Giữ nguyên những gì đã có:
