import { NavLink } from 'react-router-dom'

function MenuItem({ name, to }: { name: string; to: string }) {
  return (
    <li>
      <NavLink
        style={({ isActive }) => {
          return {
            color: isActive ? 'Highlight' : 'black'
          }
        }}
        to={to}
      >
        {name}
      </NavLink>
    </li>
  )
}

export default MenuItem
