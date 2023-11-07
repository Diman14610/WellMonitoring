import MenuItem from '@/routes/menu/menu-item'

const MENU_ITEMS = [
  { name: 'Телеметрия', to: '/telemetry' },
  { name: 'Скважины', to: '/report' },
]

function Menu({ className }: { className?: string }) {
  return (
    <ul className={className}>
      {MENU_ITEMS.map((link) => (
        <MenuItem key={link.name} to={link.to} name={link.name} />
      ))}
    </ul>
  )
}

export default Menu
